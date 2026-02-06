using REPS_backend.DTOs.Entrenamientos;
using REPS_backend.Models;
using REPS_backend.Repositories;

namespace REPS_backend.Services
{
    public class EntrenamientoService : IEntrenamientoService
    {
        private readonly IEntrenamientoRepository _entrenamientoRepository;
        private readonly IRecordPersonalService _recordService;

        public EntrenamientoService(
            IEntrenamientoRepository entrenamientoRepository,
            IRecordPersonalService recordService)
        {
            _entrenamientoRepository = entrenamientoRepository;
            _recordService = recordService;
        }

        public async Task FinalizarEntrenamientoAsync(int usuarioId, FinalizarEntrenamientoDto dto)
        {
            // 1. Guardar el entrenamiento (Historial básico)
            var entrenamiento = new Entrenamiento
            {
                UsuarioId = usuarioId,
                Nombre = dto.Nombre,
                DuracionMinutos = dto.DuracionMinutos,
                Fecha = DateTime.UtcNow
            };
            await _entrenamientoRepository.AddAsync(entrenamiento);

            // 2. Procesar Records automáticamente
            if (dto.Ejercicios != null)
            {
                foreach (var ej in dto.Ejercicios)
                {
                    if (ej.PesoMaximo > 0)
                    {
                        // Esto ya: Checkea si es record, actualiza ranking, actualiza racha
                        await _recordService.RegistrarNuevoLevantamientoAsync(usuarioId, ej.EjercicioId, ej.PesoMaximo);
                    }
                }
            }
        }
    }
}
