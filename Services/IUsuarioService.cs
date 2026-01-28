using REPS_backend.DTOs.Usuarios;

namespace REPS_backend.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioPerfilDto?> ObtenerMiPerfilAsync(int id); // Privado
        Task<UsuarioPublicoDto?> BuscarAmigoPorCodigoAsync(string codigo); // Público
        Task<bool> ActualizarPerfilAsync(int id, UsuarioUpdateDto dto);
    }
}