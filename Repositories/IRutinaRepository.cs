using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IRutinaRepository
    {
        // Obtener todas (para el listado público)
        Task<List<Rutina>> GetAllPublicasAsync(); 
        
        // Obtener una por ID (con sus ejercicios incluidos)
        Task<Rutina?> GetByIdAsync(int id);
        Task<Rutina?> GetByIdWithEjerciciosAsync(int id) => GetByIdAsync(id); // Alias/Default implementation
        
        // Obtener por Usuario
        Task<List<Rutina>> GetByUsuarioIdAsync(int usuarioId);

        // Obtener en revisión (Admin)
        Task<List<Rutina>> GetAllEnRevisionAsync();

        // Obtener todas (Admin)
        Task<List<Rutina>> GetAllAdminAsync();

        // Crear
        Task AddAsync(Rutina rutina);
        Task UpdateAsync(Rutina rutina);
        // Eliminar
        Task DeleteAsync(int id);
    }
}