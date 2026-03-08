using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;

namespace REPS_backend.Services.AI
{
    public interface IAIService
    {
        Task<string> AnalyzeWorkoutAsync(Sesion sesion);
        Task<RutinaDetalleDto> GenerateRoutineAsync(RutinaIARequestDto dto);
    }
}
