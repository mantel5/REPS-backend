using REPS_backend.DTOs.Ejercicios;
namespace REPS_backend.Services
{
    public interface IEjercicioService
    {
        Task<List<EjercicioItemDto>> ObtenerTodosParaUsuarioAsync(int userId);
        Task<EjercicioItemDto> CrearEjercicioAsync(EjercicioCreateDto dto, int? usuarioId);
        Task<bool> ActualizarEjercicioAsync(int id, EjercicioUpdateDto dto, int usuarioId); 
        Task<bool> BorrarEjercicioAsync(int id, int usuarioId);
    }
}
