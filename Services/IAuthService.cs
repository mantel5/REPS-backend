using REPS_backend.DTOs.Auth;
using System.Security.Claims;

namespace REPS_backend.Services
{
    public interface IAuthService
    {
        Task<string> RegistrarUsuarioAsync(RegisterDto dto);

        Task<string?> LoginAsync(LoginDto dto);

        bool HasAccessToResource(int requestedUserId, ClaimsPrincipal user);
    }
}