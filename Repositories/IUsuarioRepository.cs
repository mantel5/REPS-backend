using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByIdAsync(int id); 
        Task<Usuario?> GetByCodigoAmigoAsync(string codigo); 
        Task<bool> ExistsByEmailAsync(string email);
        Task CrearUsuarioAsync(Usuario usuario);
        Task UpdateUsuarioAsync(Usuario usuario);
    }
}