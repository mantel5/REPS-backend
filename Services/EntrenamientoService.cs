using REPS_backend.DTOs.Entrenamientos;
using REPS_backend.Models;
using REPS_backend.Repositories;

namespace REPS_backend.Services
{
    public class EntrenamientoService : IEntrenamientoService
    {
        private readonly IEntrenamientoRepository _entrenamientoRepository;
        private readonly IRutinaRepository _rutinaRepository;
        private readonly IRecordPersonalService _recordService;
        private readonly ILogroService _logroService;
        private readonly IRankingService _rankingService;

        public EntrenamientoService(
            IEntrenamientoRepository entrenamientoRepository,
            IRutinaRepository rutinaRepository,
            IRecordPersonalService recordService,
            ILogroService logroService,
            IRankingService rankingService)
        {
            _entrenamientoRepository = entrenamientoRepository;
            _rutinaRepository = rutinaRepository;
            _recordService = recordService;
            _logroService = logroService;
            _rankingService = rankingService;
        }

        public async Task<FinalizarResultadoDto> FinalizarEntrenamientoAsync(int usuarioId, FinalizarEntrenamientoDto dto)
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
            var recordsNuevos = new List<RecordEnSesionDto>();
            int puntosGanados = 0;
            const int PUNTOS_POR_EJERCICIO = 10;

            if (dto.Ejercicios != null)
            {
                // Track de músculos donde se batió récord (para evitar duplicar bonus)
                var musculosConRecord = new HashSet<string>();

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

                            if (serieDto.Completada && serieDto.Peso > pesoMaximoEjercicio)
                                pesoMaximoEjercicio = serieDto.Peso;
                        }
                    }

                    // Sumar puntos por ejercicio (si hay al menos una serie completada)
                    bool tieneSeriesCompletadas = ejDto.Series?.Any(s => s.Completada) ?? false;
                    if (tieneSeriesCompletadas)
                        puntosGanados += PUNTOS_POR_EJERCICIO;

                    // 3. Procesar Record Personal
                    if (pesoMaximoEjercicio > 0)
                    {
                        bool esRecord = await _recordService.RegistrarNuevoLevantamientoAsync(usuarioId, ejDto.EjercicioId, pesoMaximoEjercicio);

                        if (esRecord)
                        {
                            // Obtener info del ejercicio para devolver al frontend
                            var records = await _recordService.ObtenerRecordsUsuarioAsync(usuarioId);
                            var recordInfo = records.FirstOrDefault(r => r.EjercicioId == ejDto.EjercicioId);

                            if (recordInfo != null)
                            {
                                recordsNuevos.Add(new RecordEnSesionDto
                                {
                                    EjercicioId = ejDto.EjercicioId,
                                    EjercicioNombre = recordInfo.EjercicioNombre,
                                    GrupoMuscular = recordInfo.GrupoMuscular ?? string.Empty,
                                    PesoMaximo = recordInfo.PesoMaximo,
                                    Mejora = recordInfo.Mejora
                                });

                                // Bonus de puntos por músculo (solo 1 vez por músculo por sesión)
                                var musculo = recordInfo.GrupoMuscular ?? "Otro";
                                if (!musculosConRecord.Contains(musculo))
                                {
                                    musculosConRecord.Add(musculo);
                                    puntosGanados += 30; // Bonus por batir récord en este músculo
                                }
                            }
                        }
                    }
                }
            }

            // 4. Guardar todo (Entrenamiento + Series en cascada)
            await _entrenamientoRepository.AddAsync(entrenamiento);

            // 5. Verificar logros desbloqueados en esta sesión
            var newlyUnlockedLogros = await _logroService.CheckAndUnlockAchievementsAsync(usuarioId);

            // 6. Actualizar ranking y puntos totales del usuario
            await _rankingService.UpdateUserRankAsync(usuarioId);
            await _rankingService.UpdateStreakAsync(usuarioId);

            // 7. Actualizar pesos sugeridos en la rutina (si existe y pertenece al usuario)
            if (dto.RutinaId.HasValue && dto.RutinaId.Value > 0)
            {
                // Verificar que la rutina pertenece al usuario antes de actualizar
                var rutina = await _rutinaRepository.GetByIdAsync(dto.RutinaId.Value);
                if (rutina != null && rutina.UsuarioId == usuarioId)
                {
                    // Crear diccionario de pesos máximos por ejercicio
                    var pesosPorEjercicio = new Dictionary<int, double>();
                    
                    if (dto.Ejercicios != null)
                    {
                        foreach (var ejDto in dto.Ejercicios)
                        {
                            double pesoMaximo = 0;
                            if (ejDto.Series != null)
                            {
                                foreach (var serieDto in ejDto.Series)
                                {
                                    if (serieDto.Completada && (double)serieDto.Peso > pesoMaximo)
                                    {
                                        pesoMaximo = (double)serieDto.Peso;
                                    }
                                }
                            }
                            
                            // Solo guardar si hay al menos una serie completada
                            if (pesoMaximo > 0)
                            {
                                pesosPorEjercicio[ejDto.EjercicioId] = pesoMaximo;
                            }
                        }
                    }

                    // Actualizar los pesos sugeridos en la rutina
                    if (pesosPorEjercicio.Count > 0)
                    {
                        await _rutinaRepository.ActualizarPesosSugeridosAsync(dto.RutinaId.Value, pesosPorEjercicio);
                    }
                }
            }

            return new FinalizarResultadoDto
            {
                Mensaje = "Entrenamiento guardado y récords actualizados.",
                PuntosGanados = puntosGanados,
                RecordsPersonal = recordsNuevos,
                LogrosDesbloqueados = newlyUnlockedLogros.Select(l => new LogroEnSesionDto
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    Puntos = l.Puntos
                }).ToList()
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
    }
}
