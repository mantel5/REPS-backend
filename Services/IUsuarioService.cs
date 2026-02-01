using REPS_backend.DTOs.Usuarios;
using REPS_backend.Models;

namespace REPS_backend.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioPerfilDto?> ObtenerMiPerfilAsync(int id); 
        Task<UsuarioPublicoDto?> BuscarAmigoPorCodigoAsync(string codigo); 
        Task<bool> ActualizarPerfilAsync(int id, UsuarioUpdateDto dto);
        Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAdminAsync();
        Task<bool> CambiarEstadoBloqueoAsync(int id, bool estaActivo); 
        Task<bool> EliminarUsuarioLogicoAsync(int id);
        Task<bool> AgregarAmigoAsync(int miId, string codigoAmigo);
        Task<List<UsuarioPublicoDto>> ObtenerMisAmigosAsync(int userId);
    }
}