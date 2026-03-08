using REPS_backend.Models;
using REPS_backend.Repositories;
using REPS_backend.DTOs.Records;
using System.Threading.Tasks;

namespace REPS_backend.Services
{
    public class RecordPersonalService : IRecordPersonalService
    {
        private readonly IRecordPersonalRepository _recordRepository;
        private readonly IRankingService _rankingService;

        public RecordPersonalService(
            IRecordPersonalRepository recordRepository,
            IRankingService rankingService)
        {
            _recordRepository = recordRepository;
            _rankingService = rankingService;
        }

        public async Task<bool> RegistrarNuevoLevantamientoAsync(int userId, int ejercicioId, decimal peso)
        {
            // 1. Obtener el record actual
            var currentRecord = await _recordRepository.GetBestByExerciseAsync(userId, ejercicioId);

            bool esNuevoRecord = false;
            int puntosAOtorgar = 0;
            // TODO: Definir puntos por record en config. Por ahora hardcoded 100.
            int PUNTOS_POR_RECORD = 100;

            if (currentRecord == null)
            {
                // Primer levantamiento -> Es record
                var newRecord = new RecordPersonal
                {
                    UsuarioId = userId,
                    EjercicioId = ejercicioId,
                    PesoMaximo = peso,
                    FechaRecord = DateTime.UtcNow,
                    PesoAnterior = 0
                };
                await _recordRepository.AddAsync(newRecord);
                esNuevoRecord = true;
                puntosAOtorgar = PUNTOS_POR_RECORD; // Primer record da puntos? Asumimos que sí
            }
            else
            {
                if (currentRecord.PesoMaximo < peso)
                {
                    // Rompió el record
                    currentRecord.PesoAnterior = currentRecord.PesoMaximo;
                    currentRecord.PesoMaximo = peso;
                    currentRecord.FechaRecord = DateTime.UtcNow;
                    await _recordRepository.UpdateAsync(currentRecord);
                    esNuevoRecord = true;
                    puntosAOtorgar = PUNTOS_POR_RECORD;
                }
            }

            if (esNuevoRecord)
            {
                // Trigger puntos
                await _rankingService.AddRecordPointsAsync(userId, puntosAOtorgar);
            }

            // Siempre actualizamos el ranking por si el cambio de peso afecta al rango muscular
            await _rankingService.UpdateUserRankAsync(userId);
            await _rankingService.UpdateStreakAsync(userId);

            return esNuevoRecord;
        }

        public async Task<List<RecordPersonalDto>> ObtenerRecordsUsuarioAsync(int usuarioId)
        {
            var records = await _recordRepository.GetByUserIdAsync(usuarioId);

            return records.Select(r => new RecordPersonalDto
            {
                EjercicioId = r.EjercicioId,
                EjercicioNombre = r.Ejercicio?.Nombre ?? "Ejercicio Desconocido",
                GrupoMuscular = r.Ejercicio?.GrupoMuscular.ToString() ?? "Otro",
                PesoMaximo = r.PesoMaximo,
                Mejora = r.PesoMaximo - r.PesoAnterior,
                Fecha = r.FechaRecord,
                TiempoAtras = CalcularTiempoAtras(r.FechaRecord)
            }).OrderByDescending(r => r.Fecha).ToList();
        }

        private string CalcularTiempoAtras(DateTime fecha)
        {
            var diff = DateTime.UtcNow - fecha;
            if (diff.TotalDays < 1) return "Hoy";
            if (diff.TotalDays < 2) return "Ayer";
            if (diff.TotalDays < 7) return $"Hace {Math.Floor(diff.TotalDays)} días";
            if (diff.TotalDays < 30) return $"Hace {Math.Floor(diff.TotalDays / 7)} semanas";
            return $"Hace {Math.Floor(diff.TotalDays / 30)} meses";
        }
    }
}
