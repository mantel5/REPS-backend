using REPS_backend.DTOs.Ejercicios;
using REPS_backend.Models;
using REPS_backend.Repositories;
namespace REPS_backend.Services
{
    public class EjercicioService : IEjercicioService
    {
        private readonly IEjercicioRepository _repository;
        public EjercicioService(IEjercicioRepository repository) { _repository = repository; }

        public async Task<EjercicioItemDto> CrearEjercicioAsync(EjercicioCreateDto dto, int? usuarioId)
        {
            var nuevoEjercicio = new Ejercicio
            {
                Nombre = dto.Nombre,
                GrupoMuscular = dto.GrupoMuscular,
                DescripcionTecnica = dto.DescripcionTecnica ?? "", 
                ImagenMusculosUrl = dto.ImagenMusculosUrl ?? "",
                UsuarioCreadorId = usuarioId
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

        public async Task<bool> ActualizarEjercicioAsync(int id, EjercicioUpdateDto dto, int usuarioId)
        {
            var ejercicio = await _repository.GetByIdAsync(id);
            if (ejercicio == null) return false;

            // SEGURIDAD: Solo el dueño puede editar su ejercicio. 
            // Los ejercicios del sistema (null) NO se pueden editar por usuarios normales.
            if (ejercicio.UsuarioCreadorId != usuarioId) return false;

            ejercicio.Nombre = dto.Nombre;
            ejercicio.GrupoMuscular = dto.GrupoMuscular;
            ejercicio.DescripcionTecnica = dto.DescripcionTecnica ?? "";
            ejercicio.ImagenMusculosUrl = dto.ImagenMusculosUrl ?? "";

            await _repository.UpdateAsync(ejercicio);
            return true;
        }

        public async Task<List<EjercicioItemDto>> ObtenerTodosParaUsuarioAsync(int userId)
        {
            var ejercicios = await _repository.GetAllPersonalizadosAsync(userId);
            return ejercicios.Select(e => new EjercicioItemDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                GrupoMuscular = e.GrupoMuscular,
                ImagenMusculosUrl = e.ImagenMusculosUrl
            }).ToList();
        }

        public async Task<bool> BorrarEjercicioAsync(int id, int usuarioId)
        {
            var ejercicio = await _repository.GetByIdAsync(id);
            if (ejercicio == null) return false;
            
            // SEGURIDAD: Solo borras lo tuyo
            if (ejercicio.UsuarioCreadorId != usuarioId) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
