using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;
using REPS_backend.Repositories; // <-- Importamos Repositories
using REPS_backend.Services.AI;

namespace REPS_backend.Services
{
    public class RutinaService : IRutinaService
    {
        private readonly IRutinaRepository _repository;
        private readonly IEjercicioRepository _ejercicioRepository;
        private readonly IAIService _aiService;
        private readonly IEntrenamientoRepository? _entrenamientoRepository;

        public RutinaService(
            IRutinaRepository repository, 
            IEjercicioRepository ejercicioRepository, 
            IAIService aiService, 
            IEntrenamientoRepository? entrenamientoRepository = null)
        {
            _repository = repository;
            _ejercicioRepository = ejercicioRepository;
            _aiService = aiService;
            _entrenamientoRepository = entrenamientoRepository;
        }

        public async Task<RutinaDetalleDto> CrearRutinaAsync(RutinaCreateDto dto, int usuarioId)
        {
            // 1. Mapear DTO a Entidad
            var nuevaRutina = new Rutina
            {
                UsuarioId = usuarioId,
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                ImagenUrl = dto.ImagenUrl ?? string.Empty,
                Estado = EstadoRutina.Privada,
                Ejercicios = new List<RutinaEjercicio>()
            };

            // 2. Lógica interna (Smart Weight y Ejercicios)
            int orden = 1;
            foreach (var ejDto in dto.Ejercicios)
            {
                var ejercicioDominio = new RutinaEjercicio
                {
                    EjercicioId = ejDto.EjercicioId,
                    Orden = orden++,
                    Series = ejDto.Series,
                    Repeticiones = ejDto.Repeticiones,
                    DescansoSegundos = ejDto.DescansoSegundos,
                    Tipo = ejDto.Tipo,
                    PorcentajeDelPeso = CalcularPorcentajeSmart(ejDto.Tipo),
                    PesoSugerido = 0
                };
                nuevaRutina.Ejercicios.Add(ejercicioDominio);
            }

            // 3. Calcular duración
            nuevaRutina.DuracionMinutos = CalcularDuracionInterna(nuevaRutina.Ejercicios);

            // 4. USAR EL REPOSITORIO
            await _repository.AddAsync(nuevaRutina);

            // 5. Cargar ejercicios para el mapeo final
            var rutinaCargada = await _repository.GetByIdWithEjerciciosAsync(nuevaRutina.Id);
            return MapToDetalleDto(rutinaCargada ?? nuevaRutina);
        }

        public async Task<List<RutinaItemDto>> ObtenerRutinasPublicasAsync()
        {
            // Pedimos los datos al repositorio real
            var rutinas = await _repository.GetAllPublicasAsync();

            // Convertimos a DTO
            return rutinas.Select(r => new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Nivel = r.Nivel.ToString(),
                DuracionMinutos = r.DuracionMinutos,
                UrlImagen = r.ImagenUrl,
                CantidadEjercicios = r.Ejercicios?.Count ?? 0,
                TotalEjercicios = r.Ejercicios?.Count ?? 0,
                CreadorNombre = r.Usuario != null ? r.Usuario.Nombre : "Sistema",
                Likes = r.Likes,
                Estado = r.Estado.ToString(),
                Musculos = r.Ejercicios?
                    .Where(e => e.Ejercicio != null)
                    .Select(e => e.Ejercicio!.GrupoMuscular.ToString())
                    .Distinct()
                    .ToList() ?? new List<string>()
            }).ToList();
        }

        public async Task<List<RutinaItemDto>> ObtenerRutinasEnRevisionAsync()
        {
            var rutinas = await _repository.GetAllEnRevisionAsync();
            return rutinas.Select(r => new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Nivel = r.Nivel.ToString(),
                DuracionMinutos = r.DuracionMinutos,
                UrlImagen = r.ImagenUrl,
                CantidadEjercicios = r.Ejercicios?.Count ?? 0,
                TotalEjercicios = r.Ejercicios?.Count ?? 0,
                CreadorNombre = r.Usuario != null ? r.Usuario.Nombre : "Sistema",
                Likes = r.Likes,
                Estado = r.Estado.ToString(),
                Musculos = r.Ejercicios?
                    .Where(e => e.Ejercicio != null)
                    .Select(e => e.Ejercicio!.GrupoMuscular.ToString())
                    .Distinct()
                    .ToList() ?? new List<string>()
            }).ToList();
        }

        public async Task<List<RutinaItemDto>> ObtenerTodasRutinasAdminAsync()
        {
            var rutinas = await _repository.GetAllAdminAsync();
            return rutinas.Select(r => new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Nivel = r.Nivel.ToString(),
                DuracionMinutos = r.DuracionMinutos,
                UrlImagen = r.ImagenUrl,
                CantidadEjercicios = r.Ejercicios?.Count ?? 0,
                TotalEjercicios = r.Ejercicios?.Count ?? 0,
                CreadorNombre = r.Usuario != null ? r.Usuario.Nombre : "Sistema",
                Likes = r.Likes,
                Estado = r.Estado.ToString(),
                Musculos = r.Ejercicios?
                    .Where(e => e.Ejercicio != null)
                    .Select(e => e.Ejercicio!.GrupoMuscular.ToString())
                    .Distinct()
                    .ToList() ?? new List<string>()
            }).ToList();
        }

        public async Task<List<RutinaItemDto>> ObtenerRutinasUsuarioAsync(int usuarioId)
        {
            var rutinas = await _repository.GetByUsuarioIdAsync(usuarioId);

            return rutinas.Select(r => new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Nivel = r.Nivel.ToString(),
                DuracionMinutos = r.DuracionMinutos,
                UrlImagen = r.ImagenUrl,
                CantidadEjercicios = r.Ejercicios?.Count ?? 0,
                TotalEjercicios = r.Ejercicios?.Count ?? 0,
                CreadorNombre = "Tú",
                Likes = r.Likes,
                Estado = r.Estado.ToString(),
                Musculos = r.Ejercicios?
                    .Where(e => e.Ejercicio != null)
                    .Select(e => e.Ejercicio!.GrupoMuscular.ToString())
                    .Distinct()
                    .ToList() ?? new List<string>()
            }).ToList();
        }

        public async Task<RutinaDetalleDto> ObtenerDetalleRutinaAsync(int rutinaId, int usuarioId = 0)
        {
            var rutina = await _repository.GetByIdAsync(rutinaId);

            if (rutina == null) return null;

            // Si tenemos usuarioId, enriquecer con el último peso
            if (usuarioId > 0 && _entrenamientoRepository != null)
            {
                var entrenamientos = await _entrenamientoRepository.GetByUsuarioIdWithSeriesAsync(usuarioId);
                var ultimosPesos = entrenamientos?
                    .OrderByDescending(e => e.Fecha)
                    .SelectMany(e => e.SeriesRealizadas ?? new List<SerieLog>())
                    .Where(s => s.PesoUsado > 0)
                    .GroupBy(s => s.EjercicioId)
                    .ToDictionary(g => g.Key, g => g.First().PesoUsado);

                return MapToDetalleDto(rutina, ultimosPesos);
            }

            return MapToDetalleDto(rutina);
        }

        // --- MÉTODOS PRIVADOS (Los mismos de antes) ---
        private decimal CalcularPorcentajeSmart(TipoSerie tipo)
        {
            return tipo switch
            {
                TipoSerie.Calentamiento => 0.50m,
                TipoSerie.Aproximacion => 0.75m,
                TipoSerie.DropSet => 0.60m,
                TipoSerie.AlFallo => 0.85m,
                _ => 1.0m
            };
        }

        private int CalcularDuracionInterna(List<RutinaEjercicio> ejercicios)
        {
            if (ejercicios == null || !ejercicios.Any()) return 0;
            double segundosTotales = 0;
            foreach (var ej in ejercicios)
            {
                segundosTotales += (ej.Series * 60);
                if (ej.Series > 1) segundosTotales += (ej.Series - 1) * ej.DescansoSegundos;
            }
            segundosTotales += (ejercicios.Count * 120);
            return (int)Math.Ceiling(segundosTotales / 60);
        }

        public async Task<RutinaDetalleDto> GenerarRutinaIAAsync(RutinaIARequestDto dto, int usuarioId)
        {
            // 1. Llamar a la IA real (Gemini)
            var rutinaIA_Dto = await _aiService.GenerateRoutineAsync(dto);

            // 2. Mapear la respuesta de la IA (DTO) a una Entidad para la Base de Datos
            var rutinaEntidad = new Rutina
            {
                Nombre = rutinaIA_Dto.Nombre,
                Nivel = Enum.TryParse<NivelDificultad>(rutinaIA_Dto.Nivel, true, out var nivel) ? nivel : NivelDificultad.Intermedio,
                Estado = EstadoRutina.Privada,
                UsuarioId = usuarioId,
                EsGeneradaPorIA = true,
                Ejercicios = new List<RutinaEjercicio>()
            };

            int orden = 1;
            if (rutinaIA_Dto.Ejercicios != null)
            {
                foreach (var ejDto in rutinaIA_Dto.Ejercicios)
                {
                    var re = new RutinaEjercicio
                    {
                        EjercicioId = ejDto.EjercicioId,
                        Orden = orden++,
                        Series = ejDto.Series,
                        DescansoSegundos = 90, // Valor por defecto
                        Repeticiones = ejDto.Repeticiones.ToString(),
                        Tipo = TipoSerie.Normal,
                        PorcentajeDelPeso = 1.0m,
                        PesoSugerido = 0
                    };
                    rutinaEntidad.Ejercicios.Add(re);
                }
            }

            rutinaEntidad.DuracionMinutos = CalcularDuracionInterna(rutinaEntidad.Ejercicios.ToList());

            // 3. Guardar en BD
            await _repository.AddAsync(rutinaEntidad);

            // 4. Recargar para tener los nombres reales de los ejercicios y devolver el DTO final
            var completa = await _repository.GetByIdWithEjerciciosAsync(rutinaEntidad.Id);
            return MapToDetalleDto(completa ?? rutinaEntidad);
        }

        public async Task<int> LikeRutinaAsync(int rutinaId)
        {
            var rutina = await _repository.GetByIdAsync(rutinaId);
            if (rutina == null) return 0;

            rutina.Likes++;
            await _repository.UpdateAsync(rutina);
            return rutina.Likes;
        }

        public async Task<RutinaDetalleDto> CopiarRutinaAsync(int rutinaId, int usuarioId)
        {
            var original = await _repository.GetByIdWithEjerciciosAsync(rutinaId);
            if (original == null) throw new Exception("Rutina no encontrada");

            var copia = new Rutina
            {
                Nombre = $"{original.Nombre} (Copia)",
                Nivel = original.Nivel,
                DuracionMinutos = original.DuracionMinutos,
                Estado = EstadoRutina.Privada,
                UsuarioId = usuarioId,
                Ejercicios = original.Ejercicios?.Select(e => new RutinaEjercicio
                {
                    EjercicioId = e.EjercicioId,
                    Series = e.Series,
                    Repeticiones = e.Repeticiones,
                    DescansoSegundos = e.DescansoSegundos,
                    Orden = e.Orden,
                    Tipo = e.Tipo,
                    PorcentajeDelPeso = e.PorcentajeDelPeso,
                    PesoSugerido = e.PesoSugerido
                }).ToList() ?? new List<RutinaEjercicio>()
            };

            await _repository.AddAsync(copia);
            return MapToDetalleDto(copia);
        }

        private RutinaDetalleDto MapToDetalleDto(Rutina r, Dictionary<int, decimal>? ultimosPesos = null)
        {
            return new RutinaDetalleDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Nivel = r.Nivel.ToString(),
                DuracionMinutos = r.DuracionMinutos,
                UrlImagen = r.ImagenUrl ?? string.Empty,
                Estado = r.Estado.ToString(),
                Ejercicios = r.Ejercicios?.Select(e => new RutinaEjercicioDto
                {
                    EjercicioId = e.EjercicioId,
                    NombreEjercicio = e.Ejercicio?.Nombre ?? "Ejercicio",
                    GrupoMuscular = e.Ejercicio?.GrupoMuscular.ToString().ToUpper() ?? "OTRO",
                    Series = e.Series,
                    DescansoSegundos = e.DescansoSegundos,
                    Tipo = e.Tipo,
                    Repeticiones = e.Repeticiones,
                    UltimoPeso = ultimosPesos != null && ultimosPesos.TryGetValue(e.EjercicioId, out var peso) ? peso : 0
                }).ToList() ?? new List<RutinaEjercicioDto>()
            };
        }

        public async Task<bool> EliminarRutinaAsync(int id, int usuarioId)
        {
            var rutina = await _repository.GetByIdAsync(id);
            if (rutina == null) return false;

            // Si la aplicación requiere que sólo el creador borre, lo validamos
            if (rutina.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException("No tienes permisos para eliminar esta rutina.");

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> EliminarRutinaAdminAsync(int id)
        {
            var rutina = await _repository.GetByIdAsync(id);
            if (rutina == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> PublicarRutinaAsync(int id, int usuarioId)
        {
            var rutina = await _repository.GetByIdAsync(id);
            if (rutina == null) return false;

            if (rutina.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException("No tienes permisos para publicar esta rutina.");

            // Va a revisión en lugar de publicación directa
            rutina.Estado = EstadoRutina.EnRevision;
            await _repository.UpdateAsync(rutina);
            return true;
        }

        public async Task<bool> ValidarRutinaAsync(int id)
        {
            var rutina = await _repository.GetByIdAsync(id);
            if (rutina == null) return false;

            rutina.Estado = EstadoRutina.Publicada;
            await _repository.UpdateAsync(rutina);
            return true;
        }

        public async Task<bool> RechazarRutinaAsync(int id)
        {
            var rutina = await _repository.GetByIdAsync(id);
            if (rutina == null) return false;

            rutina.Estado = EstadoRutina.Rechazada;
            await _repository.UpdateAsync(rutina);
            return true;
        }

        public async Task<RutinaDetalleDto> ActualizarRutinaAsync(int id, RutinaCreateDto dto, int usuarioId)
        {
            var rutina = await _repository.GetByIdWithEjerciciosAsync(id);
            if (rutina == null) throw new Exception("Rutina no encontrada.");

            if (rutina.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException("No tienes permisos para editar esta rutina.");

            // 1. Actualizar datos básicos
            rutina.Nombre = dto.Nombre;
            rutina.Nivel = dto.Nivel;
            if (dto.ImagenUrl != null)
                rutina.ImagenUrl = dto.ImagenUrl;

            // 2. Limpiar ejercicios anteriores e insertar los nuevos
            rutina.Ejercicios.Clear();

            int orden = 1;
            if (dto.Ejercicios != null)
            {
                foreach (var ejDto in dto.Ejercicios)
                {
                    var ejercicioDominio = new RutinaEjercicio
                    {
                        EjercicioId = ejDto.EjercicioId,
                        Orden = orden++,
                        Series = ejDto.Series,
                        Repeticiones = ejDto.Repeticiones,
                        DescansoSegundos = ejDto.DescansoSegundos,
                        Tipo = ejDto.Tipo,
                        PorcentajeDelPeso = CalcularPorcentajeSmart(ejDto.Tipo),
                        PesoSugerido = 0
                    };
                    rutina.Ejercicios.Add(ejercicioDominio);
                }
            }

            // 3. Recalcular duración
            rutina.DuracionMinutos = CalcularDuracionInterna(rutina.Ejercicios.ToList());

            // 4. Guardar cambios
            await _repository.UpdateAsync(rutina);

            // 5. Cargar completa para DTO final
            var completa = await _repository.GetByIdWithEjerciciosAsync(id);
            return MapToDetalleDto(completa ?? rutina);
        }
    }
}
