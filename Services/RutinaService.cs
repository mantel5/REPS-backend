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

        public RutinaService(
            IRutinaRepository rutinaRepository,
            IEjercicioRepository ejercicioRepository,
            IEntrenamientoRepository entrenamientoRepository)
        {
            _rutinaRepository = rutinaRepository;
            _ejercicioRepository = ejercicioRepository;
            _entrenamientoRepository = entrenamientoRepository;
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

            if (dto.EjerciciosIds != null && dto.EjerciciosIds.Any())
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
    }
}