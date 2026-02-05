using Microsoft.AspNetCore.Mvc;
using REPS_backend.DTOs.Auth;
using REPS_backend.Services;

namespace REPS_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<object>> Register(RegisterDto dto)
        {
            try
            {
                var token = await _authService.RegistrarUsuarioAsync(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null) return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            
            return Ok(new { token });
        }
    }
}