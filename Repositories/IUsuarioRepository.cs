using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);

        Task<bool> ExistsByEmailAsync(string email);

        Task CrearUsuarioAsync(Usuario usuario);
    }
}