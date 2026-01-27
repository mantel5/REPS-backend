using REPS_backend.DTOs.Ejercicios;
using REPS_backend.Models;
using REPS_backend.Repositories;
namespace REPS_backend.Services
{
    public class EjercicioService : IEjercicioService
    {
        private readonly IEjercicioRepository _repository;
        public EjercicioService(IEjercicioRepository repository)
        {
            _repository = repository;
        }

        public async Task<EjercicioItemDto> CrearEjercicioAsync(EjercicioCreateDto dto, int? usuarioId)
        {
            var nuevoEjercicio = new Ejercicio
            {
                Nombre = dto.Nombre,
                GrupoMuscular = dto.GrupoMuscular,
                DescripcionTecnica = dto.DescripcionTecnica ?? "", 
                ImagenMusculosUrl = dto.ImagenMusculosUrl ?? "",
                UsuarioCreadorId = usuarioId // Aquí guardamos quién lo creó
            };
            await _repository.AddAsync(nuevoEjercicio);
            return new EjercicioItemDto
            {
                Id = nuevoEjercicio.Id,
                Nombre = nuevoEjercicio.Nombre,
                GrupoMuscular = nuevoEjercicio.GrupoMuscular,
                ImagenMusculosUrl = nuevoEjercicio.ImagenMusculosUrl
            };
        }

        public async Task<List<EjercicioItemDto>> ObtenerTodosParaUsuarioAsync(int userId)
        {
            // Pasamos el ID al repositorio para que filtre
            var ejercicios = await _repository.GetAllPersonalizadosAsync(userId);

            return ejercicios.Select(e => new EjercicioItemDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                GrupoMuscular = e.GrupoMuscular,
                ImagenMusculosUrl = e.ImagenMusculosUrl
            }).ToList();
        }

        public async Task<bool> BorrarEjercicioAsync(int id)
        {
            // AQUÍ PODRÍAS AÑADIR SEGURIDAD EXTRA:
            // Verificar que el ejercicio pertenezca al usuario antes de borrar
            var ejercicio = await _repository.GetByIdAsync(id);
            if (ejercicio == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
