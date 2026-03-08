using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByIdAsync(int id); 
        Task<Usuario?> GetByCodigoAmigoAsync(string codigo); 
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetAllAsync(); 
        Task CrearUsuarioAsync(Usuario usuario);
        Task UpdateUsuarioAsync(Usuario usuario);
        Task<bool> SonAmigosAsync(int usuarioId1, int usuarioId2);
        Task AgregarAmistadAsync(Amistad amistad);
        Task<IEnumerable<Usuario>> GetAmigosDeUsuarioAsync(int usuarioId);
        Task<IEnumerable<Usuario>> GetSolicitudesPendientesAsync(int userId);
        Task<Amistad?> GetAmistadEntreUsuariosAsync(int id1, int id2);
        Task EliminarAmistadAsync(Amistad amistad);
        Task UpdateAmistadAsync(Amistad amistad);
        
    }
}