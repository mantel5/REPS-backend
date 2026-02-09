using REPS_backend.DTOs.Entrenamientos;

namespace REPS_backend.Services
{
    public interface IEntrenamientoService
    {
        Task FinalizarEntrenamientoAsync(int usuarioId, FinalizarEntrenamientoDto dto);
        Task<List<EntrenamientoHistorialDto>> ObtenerHistorialUsuarioAsync(int usuarioId);
    }
}
