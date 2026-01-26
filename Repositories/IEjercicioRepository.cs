using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IEjercicioRepository
    {
        Task<List<Ejercicio>> GetAllAsync();
        Task<Ejercicio?> GetByIdAsync(int id);
        Task<Ejercicio> AddAsync(Ejercicio ejercicio);
    }
}