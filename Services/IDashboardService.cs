using REPS_backend.DTOs.Dashboard;

namespace REPS_backend.Services
{
    public interface IDashboardService
    {
        Task<DashboardResumenDto> ObtenerResumenHomeAsync(int userId);
    }
}
