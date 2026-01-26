using REPS_backend.DTOs.Auth;

namespace REPS_backend.Services
{
    public interface IAuthService
    {
        Task<string> RegistrarUsuarioAsync(RegisterDto dto);

        Task<string?> LoginAsync(LoginDto dto);
    }
}