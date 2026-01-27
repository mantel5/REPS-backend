using REPS_backend.Models;
namespace REPS_backend.Repositories
{
    public interface IRutinaEjercicioRepository
    {
        Task<RutinaEjercicio?> GetByIdAsync(int id);
        Task AddAsync(RutinaEjercicio rutinaEjercicio);
        Task UpdateAsync(RutinaEjercicio rutinaEjercicio);
        Task DeleteAsync(int id);
    }
}
