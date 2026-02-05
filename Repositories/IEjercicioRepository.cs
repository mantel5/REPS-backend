using REPS_backend.Models;
namespace REPS_backend.Repositories
{
    public interface IEjercicioRepository
    {
        Task<List<Ejercicio>> GetAllPersonalizadosAsync(int userId);
        Task<Ejercicio?> GetByIdAsync(int id);
        Task AddAsync(Ejercicio ejercicio);
        Task UpdateAsync(Ejercicio ejercicio);
        Task DeleteAsync(int id);
    }
}
