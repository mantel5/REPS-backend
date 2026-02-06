using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IEntrenamientoRepository
    {
        Task AddAsync(Entrenamiento entrenamiento);
        Task<int> CountByUsuarioIdAsync(int usuarioId);
        // Add more methods as needed (GetHistory, etc.)
    }
}
