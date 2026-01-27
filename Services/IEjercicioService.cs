using REPS_backend.DTOs.Ejercicios;
namespace REPS_backend.Services
{
    public interface IEjercicioService
    {
        // AHORA PIDE EL ID
        Task<List<EjercicioItemDto>> ObtenerTodosParaUsuarioAsync(int userId);
        Task<EjercicioItemDto> CrearEjercicioAsync(EjercicioCreateDto dto, int? usuarioId);
        Task<bool> BorrarEjercicioAsync(int id);
    }
}
