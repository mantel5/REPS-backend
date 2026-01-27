using REPS_backend.Models;
namespace REPS_backend.Repositories
{
    public interface IRutinaRepository
    {
        Task<List<Rutina>> GetAllPublicasAsync();
        Task<Rutina?> GetByIdWithEjerciciosAsync(int id);
        Task<Rutina?> GetByIdSimpleAsync(int id); 
        Task AddAsync(Rutina rutina);
        Task UpdateAsync(Rutina rutina); 
        Task DeleteAsync(int id);
    }
}
