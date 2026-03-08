using REPS_backend.DTOs.Rutinas;
namespace REPS_backend.Services
{
    public interface IRutinaEjercicioService
    {
        Task<bool> AgregarEjercicioARutinaAsync(RutinaEjercicioAddDto dto, int usuarioId);
        Task<bool> ActualizarRutinaEjercicioAsync(int id, RutinaEjercicioUpdateDto dto, int usuarioId);
        Task<bool> EliminarEjercicioDeRutinaAsync(int id, int usuarioId);
    }
}
