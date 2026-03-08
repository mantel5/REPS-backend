using REPS_backend.DTOs.Entrenamientos;
using REPS_backend.DTOs.Records;
using REPS_backend.DTOs.Logros;

namespace REPS_backend.Services
{
    public interface IEntrenamientoService
    {
        Task<FinalizarResultadoDto> FinalizarEntrenamientoAsync(int usuarioId, FinalizarEntrenamientoDto dto);
        Task<List<EntrenamientoHistorialDto>> ObtenerHistorialUsuarioAsync(int usuarioId);
    }
}
