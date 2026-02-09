using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IEntrenamientoRepository
    {
        Task AddAsync(Entrenamiento entrenamiento);
        Task<int> CountByUsuarioIdAsync(int usuarioId);
        Task<List<Entrenamiento>> GetByUsuarioIdWithSeriesAsync(int usuarioId);
        Task<List<Entrenamiento>> GetByUsuarioIdAsync(int usuarioId);
        Task<Dictionary<int, DateTime>> ObtenerUltimasFechasRutinasAsync(int usuarioId, List<int> rutinaIds);
    }
}
