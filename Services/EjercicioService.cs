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
            // Convertimos el DTO (caja de entrada) a Entidad (Base de datos)
            var nuevoEjercicio = new Ejercicio
            {
                Nombre = dto.Nombre,
                GrupoMuscular = dto.GrupoMuscular,
                // Usamos "??" para poner texto vacío si viene null
                DescripcionTecnica = dto.DescripcionTecnica ?? "", 
                ImagenMusculosUrl = dto.ImagenMusculosUrl ?? "",
                UsuarioCreadorId = usuarioId
            };

            await _repository.AddAsync(nuevoEjercicio);

            // Devolvemos el resultado en formato DTO
            return new EjercicioItemDto
            {
                Id = nuevoEjercicio.Id,
                Nombre = nuevoEjercicio.Nombre,
                GrupoMuscular = nuevoEjercicio.GrupoMuscular,
                ImagenMusculosUrl = nuevoEjercicio.ImagenMusculosUrl
            };
        }

        public async Task<List<EjercicioItemDto>> ObtenerTodosAsync()
        {
            var ejercicios = await _repository.GetAllAsync();

            // Convertimos la lista de Entidades a lista de DTOs
            return ejercicios.Select(e => new EjercicioItemDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                GrupoMuscular = e.GrupoMuscular,
                ImagenMusculosUrl = e.ImagenMusculosUrl
            }).ToList();
        }
    }
}