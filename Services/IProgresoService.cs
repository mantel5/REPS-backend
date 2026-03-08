using REPS_backend.DTOs.Progreso;

namespace REPS_backend.Services
{
    public interface IProgresoService
    {
        Task<List<ProgresoMuscularDto>> ObtenerProgresoMuscularAsync(int usuarioId);
        Task<ProgresoGeneralDto> ObtenerProgresoGeneralAsync(int usuarioId);
        Task<AnaliticaDto> ObtenerAnaliticaAsync(int usuarioId);
    }
}
