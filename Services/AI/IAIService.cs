using REPS_backend.DTOs.AI;
using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;

namespace REPS_backend.Services.AI
{
    public interface IAIService
    {
        Task<string> AnalyzeWorkoutAsync(REPS_backend.Models.Sesion sesion);
        Task<RutinaDetalleDto> GenerateRoutineAsync(RutinaGeneracionDto dto);
    }
}
