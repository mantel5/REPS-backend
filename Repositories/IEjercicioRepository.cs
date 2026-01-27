using REPS_backend.Models;
namespace REPS_backend.Repositories
{
    public interface IEjercicioRepository
    {
        // AHORA PIDE EL ID DEL USUARIO PARA FILTRAR
        Task<List<Ejercicio>> GetAllPersonalizadosAsync(int userId); 
        Task<Ejercicio?> GetByIdAsync(int id);
        Task AddAsync(Ejercicio ejercicio);
        Task DeleteAsync(int id);
    }
}
