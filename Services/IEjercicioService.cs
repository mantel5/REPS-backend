using REPS_backend.DTOs.Ejercicios;
namespace REPS_backend.Services
{
    public interface IEjercicioService
    {
        Task<List<EjercicioItemDto>> ObtenerTodosAsync();
        Task<EjercicioItemDto> CrearEjercicioAsync(EjercicioCreateDto dto, int? usuarioId);
        Task<bool> BorrarEjercicioAsync(int id);
    }
}
