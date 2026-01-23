using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IRutinaRepository
    {
        Task<List<Rutina>> GetAllPublicasAsync(); 
        
        Task<Rutina?> GetByIdAsync(int id);
        
        Task AddAsync(Rutina rutina);
        
        // (Opcional por ahora: Update y Delete)
    }
}