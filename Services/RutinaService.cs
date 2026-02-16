using REPS_backend.DTOs.Rutinas;
using REPS_backend.DTOs.Ejercicios;
using REPS_backend.Models;
using REPS_backend.Repositories;

namespace REPS_backend.Services
{
    public class RutinaService : IRutinaService
    {
        private readonly IRutinaRepository _rutinaRepository;
        private readonly IEjercicioRepository _ejercicioRepository;
        private readonly IEntrenamientoRepository _entrenamientoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public RutinaService(
            IRutinaRepository rutinaRepository,
            IEjercicioRepository ejercicioRepository,
            IEntrenamientoRepository entrenamientoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _rutinaRepository = rutinaRepository;
            _ejercicioRepository = ejercicioRepository;
            _entrenamientoRepository = entrenamientoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<RutinaDetalleDto> CrearRutinaAsync(RutinaCreateDto dto, int usuarioId)
        {
            var nuevaRutina = new Rutina
            {
                Nombre = dto.Nombre,
                UsuarioId = usuarioId,
                EsPublica = false,
                Estado = EstadoRutina.Privada,
                Ejercicios = new List<RutinaEjercicio>()
            };

            if (dto.Ejercicios != null && dto.Ejercicios.Any())
            {
                int orden = 1;
                foreach (var ejDto in dto.Ejercicios)
                {
                    var ejercicio = await _ejercicioRepository.GetByIdAsync(ejDto.EjercicioId);
                    if (ejercicio != null)
                    {
                        nuevaRutina.Ejercicios.Add(new RutinaEjercicio
                        {
                            EjercicioId = ejDto.EjercicioId,
                            Orden = orden++,
                            Series = ejDto.Series > 0 ? ejDto.Series : 3,
                            Repeticiones = !string.IsNullOrEmpty(ejDto.Repeticiones) ? ejDto.Repeticiones : "10-12",
                            DescansoSegundos = 60,
                            Tipo = TipoSerie.Normal
                        });
                    }
                }
            }
            else if (dto.EjerciciosIds != null && dto.EjerciciosIds.Any())
            {
                int orden = 1;
                foreach (var ejercicioId in dto.EjerciciosIds)
                {
                    var ejercicio = await _ejercicioRepository.GetByIdAsync(ejercicioId);
                    if (ejercicio != null)
                    {
                        nuevaRutina.Ejercicios.Add(new RutinaEjercicio
                        {
                            EjercicioId = ejercicioId,
                            Orden = orden++,
                            Series = 3,
                            Repeticiones = "10-12",
                            DescansoSegundos = 60,
                            Tipo = TipoSerie.Normal
                        });
                    }
                }
            }

            nuevaRutina.CalcularDuracionEstimada();

            await _rutinaRepository.AddAsync(nuevaRutina);

            return await ObtenerDetalleRutinaAsync(nuevaRutina.Id) ?? new RutinaDetalleDto();
        }

        public async Task<bool> ActualizarRutinaAsync(int id, RutinaUpdateDto dto, int usuarioId)
        {
            var rutina = await _rutinaRepository.GetByIdSimpleAsync(id);
            if (rutina == null) return false;

            if (rutina.UsuarioId != usuarioId) return false;

            if (rutina.Estado == EstadoRutina.Publicada || rutina.Estado == EstadoRutina.EnRevision)
            {
                rutina.Estado = EstadoRutina.Privada;
                rutina.EsPublica = false;
            }

            rutina.Nombre = dto.Nombre;
            rutina.Descripcion = dto.Descripcion;

            await _rutinaRepository.UpdateAsync(rutina);
            return true;
        }

        public async Task<List<RutinaItemDto>> ObtenerRutinasPublicasAsync()
        {
            var rutinas = await _rutinaRepository.GetAllPublicasAsync();
            return rutinas.Select(r => new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                CreadorNombre = r.Usuario != null ? r.Usuario.Nombre : "Desconocido",
                TotalEjercicios = r.Ejercicios.Count,
                Likes = r.Likes
            }).ToList();
        }

        public async Task<RutinaDetalleDto?> ObtenerDetalleRutinaAsync(int id)
        {
            var r = await _rutinaRepository.GetByIdWithEjerciciosAsync(id);
            if (r == null) return null;

            return new RutinaDetalleDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                CreadorNombre = r.Usuario != null ? r.Usuario.Nombre : "Desconocido",
                Likes = r.Likes,
                Ejercicios = r.Ejercicios.Select(re => new EjercicioEnRutinaDto
                {
                    EjercicioId = re.EjercicioId,
                    Nombre = re.Ejercicio?.Nombre ?? "Ejercicio no encontrado",
                    Series = re.Series,
                    Repeticiones = re.Repeticiones
                }).ToList()
            };
        }

        public async Task<bool> BorrarRutinaAsync(int id, int usuarioId)
        {
            var rutina = await _rutinaRepository.GetByIdSimpleAsync(id);
            if (rutina == null) return false;

            if (rutina.UsuarioId != usuarioId) return false;

            await _rutinaRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> EnviarARevisionAsync(int rutinaId, int usuarioId)
        {
            var rutina = await _rutinaRepository.GetByIdSimpleAsync(rutinaId);

            if (rutina == null) return false;
            if (rutina.UsuarioId != usuarioId) return false;

            if (rutina.Estado == EstadoRutina.Baneada) return false;

            if (rutina.Estado == EstadoRutina.Publicada || rutina.Estado == EstadoRutina.EnRevision) return false;

            rutina.Estado = EstadoRutina.EnRevision;
            await _rutinaRepository.UpdateAsync(rutina);
            return true;
        }

        public async Task<IEnumerable<Rutina>> ObtenerRutinasPendientesAsync()
        {
            var todas = await _rutinaRepository.GetAllAsync();
            return todas.Where(r => r.Estado == EstadoRutina.EnRevision).ToList();
        }

        public async Task<bool> ValidarRutinaAsync(int rutinaId, bool aprobar)
        {
            var rutina = await _rutinaRepository.GetByIdSimpleAsync(rutinaId);
            if (rutina == null) return false;

            if (aprobar)
            {
                rutina.Estado = EstadoRutina.Publicada;
                rutina.EsPublica = true;
            }
            else
            {
                rutina.Estado = EstadoRutina.Rechazada;
                rutina.EsPublica = false;
            }

            await _rutinaRepository.UpdateAsync(rutina);
            return true;
        }

        public async Task<bool> BanearRutinaAsync(int rutinaId)
        {
            var rutina = await _rutinaRepository.GetByIdSimpleAsync(rutinaId);
            if (rutina == null) return false;

            rutina.Estado = EstadoRutina.Baneada;
            rutina.EsPublica = false;

            await _rutinaRepository.UpdateAsync(rutina);
            return true;
        }

        public async Task<List<RutinaItemDto>> ObtenerRutinasDeUsuarioAsync(int usuarioId, NivelDificultad? nivel = null, GrupoMuscular? musculo = null)
        {
            var rutinas = await _rutinaRepository.GetByUsuarioIdAsync(usuarioId, nivel, musculo);

            if (!rutinas.Any()) return new List<RutinaItemDto>();

            var rutinaIds = rutinas.Select(r => r.Id).ToList();
            var ultimasFechas = await _entrenamientoRepository.ObtenerUltimasFechasRutinasAsync(usuarioId, rutinaIds);

            return rutinas.Select(r => new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                CreadorNombre = "Tú",
                Likes = r.Likes,
                TotalEjercicios = r.Ejercicios?.Count ?? 0,
                UltimaVezRealizada = ultimasFechas.ContainsKey(r.Id) ? ultimasFechas[r.Id] : null
            }).ToList();
        }
        public async Task<bool> ToggleLikeAsync(int rutinaId, int usuarioId)
        {
            var existingLike = await _rutinaRepository.ObtenerLikeAsync(rutinaId, usuarioId);

            if (existingLike != null)
            {
                await _rutinaRepository.RemoveLikeAsync(existingLike);
                return false;
            }
            else
            {
                var newLike = new Like
                {
                    RutinaId = rutinaId,
                    UsuarioId = usuarioId,
                    FechaLike = DateTime.UtcNow
                };
                await _rutinaRepository.AddLikeAsync(newLike);
                return true;
            }
        }

        public async Task<List<RutinaItemDto>> ObtenerRutinasIAAsync(int solicitarUserId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(solicitarUserId);
            if (usuario == null) return new List<RutinaItemDto>();

            // VALIDACIÓN DE ROL/SUSCRIPCIÓN
            // Solo dejamos si es Pro o Admin
            // (Asumimos que Rol.Admin es "Admin", ajusta si usas constantes)
            bool esAdmin = usuario.Rol == Rol.Admin;
            if (!usuario.EsPro() && !esAdmin)
            {
                // Podríamos lanzar excepción o devolver lista vacía. 
                // Devolver lista vacía es más seguro para no romper el front abruptamente, aunque una excepción 403 sería más HTTP-correcta.
                // Aquí lanzaré una excepción controlada para que el controller devuelva 403.
                throw new UnauthorizedAccessException("Esta funcionalidad es exclusiva para usuarios Pro.");
            }

            var todas = await _rutinaRepository.GetAllPublicasAsync();

            // Filtramos las creadas por "AI Trainer"
            // Ojo: En el seed o al crear rutinas con IA, asegúrate de ponerle este nombre al "CreadorNombre" si no tiene usuario real,
            // O si tiene un usuario real (un admin bot), filtramos por ese ID.
            // Según tu código actual en GeminiService: rutina.CreadorNombre = "AI Trainer";
            // Pero recuerda que "CreadorNombre" en RutinaDetalleDto es dinámico. En la BBDD la Rutina tiene UsuarioId.
            // Si las rutinas IA no se guardan con un UsuarioId especial, será difícil filtrarlas.
            // ASUNCIÓN: Las rutinas IA se guardan sin UsuarioId (null) o con un UsuarioId de sistema.
            // REVISIÓN: Tu endpoint de IA 'GenerarRutinaIA' devuelve el DTO pero NO LA GUARDA en BBDD automáticamente.
            // El usuario debe guardarla. Si la guarda, pasa a ser SUYA.
            // PERO el usuario pide "Ver rutinas generadas por la IA".
            // ESTO SIGNIFICA: Rutinas "plantilla" creadas por la IA y hechas públicas por el Admin.
            // O rutinas que el sistema genera y deja públicas.
            // Vamos a filtrar por aquellas cuyo Usuario tenga nombre "AI Trainer" o similar. 
            // Si no existe tal usuario, buscaremos por string en Nombre o similar.

            // PLAN B: Filtrar rutinas públicas donde el usuario creador se llame "AI Trainer"
            // Para eso necesitamos que exista un usuario "AI Trainer" en la BBDD.

            return todas.Where(r => r.Usuario != null && r.Usuario.Nombre == "AI Trainer")
                        .Select(r => new RutinaItemDto
                        {
                            Id = r.Id,
                            Nombre = r.Nombre,
                            CreadorNombre = "AI Trainer", // r.Usuario.Nombre
                            TotalEjercicios = r.Ejercicios.Count,
                            Likes = r.Likes
                        }).ToList();
        }

        public async Task<List<RutinaItemDto>> ObtenerRutinasGuardadasAsync(int usuarioId)
        {
            // Necesitamos un método en el repo para obtener las rutinas dadas like por el usuario
            // Como no lo tenemos en la interfaz básica, tendremos que añadirlo o hacerlo en memoria (ineficiente pero rápido ahora).
            // Lo ideal: _rutinaRepository.GetLikedByUserIdAsync(usuarioId);
            // Por ahora, lo haré obteniendo todas las públicas y filtrando en memoria si tienen Like de este user.
            // Esto es LENTO si hay muchas rutinas. 
            // MEJORA: Añadir método al repositorio.

            // VARIANTE RÁPIDA (Check plan): El plan decía "GET endpoint to retrieve liked routines".
            // Vamos a añadir el método al repositorio en el siguiente paso para hacerlo bien.
            // Por ahora dejo el placeholder llamando al repo (que implementaremos luego).
            var rutinas = await _rutinaRepository.GetLikedByUserIdAsync(usuarioId);

            return rutinas.Select(r => new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                CreadorNombre = r.Usuario?.Nombre ?? "Desconocido",
                TotalEjercicios = r.Ejercicios.Count,
                Likes = r.Likes,
                EsLikeado = true // Obvio
            }).ToList();
        }

        public async Task<RutinaDetalleDto?> ImportarRutinaAsync(int rutinaId, int usuarioId)
        {
            var rutinaOriginal = await _rutinaRepository.GetByIdWithEjerciciosAsync(rutinaId);
            if (rutinaOriginal == null) return null;

            // Clonar
            var nuevaRutina = new Rutina
            {
                Nombre = $"{rutinaOriginal.Nombre} (Copia)",
                Descripcion = rutinaOriginal.Descripcion,
                UsuarioId = usuarioId,
                EsPublica = false,
                Estado = EstadoRutina.Privada,
                Nivel = rutinaOriginal.Nivel,
                DuracionMinutos = rutinaOriginal.DuracionMinutos,
                // GrupoMuscularPrincipal, Material, FechaCreacion, Estilo no existen en el modelo actual
                Ejercicios = new List<RutinaEjercicio>()
            };

            foreach (var ej in rutinaOriginal.Ejercicios)
            {
                nuevaRutina.Ejercicios.Add(new RutinaEjercicio
                {
                    EjercicioId = ej.EjercicioId,
                    Orden = ej.Orden,
                    Series = ej.Series,
                    Repeticiones = ej.Repeticiones,
                    DescansoSegundos = ej.DescansoSegundos,
                    Tipo = ej.Tipo
                    // PesoObjetivo, RIR, Cadencia, Notas no existen en el modelo actual
                });
            }

            await _rutinaRepository.AddAsync(nuevaRutina);

            return await ObtenerDetalleRutinaAsync(nuevaRutina.Id);
        }

    }
}