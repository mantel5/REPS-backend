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
            // 1. Crear el entrenamiento
            var entrenamiento = new Entrenamiento
            {
                UsuarioId = usuarioId,
                RutinaId = dto.RutinaId,
                Nombre = dto.Nombre,
                DuracionMinutos = dto.DuracionMinutos,
                Fecha = DateTime.UtcNow,
                SeriesRealizadas = new List<SerieLog>()
            };

            // 2. Procesar ejercicios y series
            if (dto.Ejercicios != null)
            {
                foreach (var ejDto in dto.Ejercicios)
                {
                    decimal pesoMaximoEjercicio = 0;

                    if (ejDto.Series != null)
                    {
                        foreach (var serieDto in ejDto.Series)
                        {
                            var serieLog = new SerieLog
                            {
                                EjercicioId = ejDto.EjercicioId,
                                NumeroSerie = serieDto.NumeroSerie,
                                PesoUsado = serieDto.Peso,
                                RepsRealizadas = serieDto.Reps,
                                Completada = serieDto.Completada
                            };
                            entrenamiento.SeriesRealizadas.Add(serieLog);

                            // Actualizar máximo local
                            if (serieDto.Peso > pesoMaximoEjercicio)
                            {
                                pesoMaximoEjercicio = serieDto.Peso;
                            }
                        }
                    }

                    // 3. Procesar Record Personal si hubo levantamiento
                    if (pesoMaximoEjercicio > 0)
                    {
                        await _recordService.RegistrarNuevoLevantamientoAsync(usuarioId, ejDto.EjercicioId, pesoMaximoEjercicio);
                    }
                }
            }

            // 4. Guardar todo (Entrenamiento + Series en cascada)
            await _entrenamientoRepository.AddAsync(entrenamiento);
        }
        public async Task<List<EntrenamientoHistorialDto>> ObtenerHistorialUsuarioAsync(int usuarioId)
        {
            var entrenamientos = await _entrenamientoRepository.GetByUsuarioIdWithSeriesAsync(usuarioId);

            return entrenamientos.Select(e => new EntrenamientoHistorialDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Fecha = e.Fecha,
                DuracionMinutos = e.DuracionMinutos,
                Ejercicios = e.SeriesRealizadas
                    .GroupBy(s => s.EjercicioId)
                    .Select(g => new EjercicioHistorialDto
                    {
                        EjercicioId = g.Key,
                        NombreEjercicio = g.First().Ejercicio?.Nombre ?? "Ejercicio " + g.Key,
                        Series = g.Select(s => new SerieDto
                        {
                            NumeroSerie = s.NumeroSerie,
                            Peso = s.PesoUsado,
                            Reps = s.RepsRealizadas,
                            Completada = s.Completada
                        }).ToList()
                    }).ToList()
            }).ToList();
        }
    }
}
