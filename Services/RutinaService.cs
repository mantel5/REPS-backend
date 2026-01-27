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
        public RutinaService(IRutinaRepository rutinaRepository, IEjercicioRepository ejercicioRepository)
        {
            _rutinaRepository = rutinaRepository;
            _ejercicioRepository = ejercicioRepository;
        }

        public async Task<RutinaDetalleDto> CrearRutinaAsync(RutinaCreateDto dto, int usuarioId)
        {
            var nuevaRutina = new Rutina
            {
                Nombre = dto.Nombre,
                UsuarioId = usuarioId,
                EsPublica = true,
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
            
            // SEGURIDAD: Solo el dueño puede editar
            if (rutina.UsuarioId != usuarioId) return false;

            rutina.Nombre = dto.Nombre;
            rutina.Descripcion = dto.Descripcion;
            rutina.EsPublica = dto.EsPublica;

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
                TotalEjercicios = r.Ejercicios.Count
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
    }
}
