using REPS_backend.DTOs.Entrenamientos;

namespace REPS_backend.Services
{
    public interface IEntrenamientoService
    {
        Task<EntrenamientoResultadoDto> FinalizarEntrenamientoAsync(int usuarioId, FinalizarEntrenamientoDto dto);
        Task<List<EntrenamientoHistorialDto>> ObtenerHistorialUsuarioAsync(int usuarioId);
        Task<EntrenamientoInitDto?> IniciarEntrenamientoAsync(int rutinaId);
    }
}
