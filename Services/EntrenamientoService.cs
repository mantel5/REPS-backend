using REPS_backend.DTOs.Entrenamientos;
using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;
using REPS_backend.Repositories;

namespace REPS_backend.Services
{
    public class EntrenamientoService : IEntrenamientoService
    {
        private readonly IEntrenamientoRepository _entrenamientoRepository;
        private readonly IRecordPersonalService _recordService;
        private readonly REPS_backend.Services.AI.IAIService _aiService;
        private readonly IRutinaRepository _rutinaRepository;

        public EntrenamientoService(
            IEntrenamientoRepository entrenamientoRepository,
            IRecordPersonalService recordService,
            REPS_backend.Services.AI.IAIService aiService,
            IRutinaRepository rutinaRepository)
        {
            _entrenamientoRepository = entrenamientoRepository;
            _recordService = recordService;
            _aiService = aiService;
            _rutinaRepository = rutinaRepository;
        }

        public async Task<EntrenamientoResultadoDto> FinalizarEntrenamientoAsync(int usuarioId, FinalizarEntrenamientoDto dto)
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

            // 5. Generar consejo IA
            // Mapeamos el entrenamiento a Sesion para el servicio de IA (o usamos Entrenamiento directamente si cambiamos la interfaz)
            // Como Sesion es el modelo base que usa IAIService, lo mapeamos.
            var sesionParaIA = new Sesion 
            { 
               NombreRutinaSnapshot = entrenamiento.Nombre,
               DuracionRealMinutos = entrenamiento.DuracionMinutos,
               SeriesRealizadas = entrenamiento.SeriesRealizadas
            };

            var consejo = await _aiService.AnalyzeWorkoutAsync(sesionParaIA);

            return new EntrenamientoResultadoDto
            {
                EntrenamientoId = entrenamiento.Id,
                Mensaje = "Entrenamiento completado con éxito.",
                ConsejoIA = consejo
            };
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

        public async Task<EntrenamientoInitDto?> IniciarEntrenamientoAsync(int rutinaId)
        {
            var rutina = await _rutinaRepository.GetByIdWithEjerciciosAsync(rutinaId);
            if (rutina == null) return null;

            return new EntrenamientoInitDto
            {
                RutinaId = rutina.Id,
                Nombre = rutina.Nombre,
                DuracionEstimadaMinutos = rutina.DuracionMinutos,
                Ejercicios = rutina.Ejercicios.Select(re => new EntrenamientoEjercicioInitDto
                {
                    EjercicioId = re.EjercicioId,
                    NombreEjercicio = re.Ejercicio?.Nombre ?? "Ejercicio",
                    ImagenMusculosUrl = re.Ejercicio?.ImagenMusculosUrl ?? "",
                    SeriesObjetivo = re.Series,
                    RepeticionesObjetivo = re.Repeticiones,
                    DescansoSegundos = re.DescansoSegundos,
                    PesoSugerido = re.PesoSugerido,
                    Tipo = re.Tipo
                }).ToList()
            };
        }
    }
}
