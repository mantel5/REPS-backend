using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REPS_backend.DTOs.Usuarios;
using REPS_backend.Services;
using System.Security.Claims;

namespace REPS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // 1. OBTENER MI PERFIL
        [HttpGet("perfil")]
        public async Task<IActionResult> GetMiPerfil()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var perfil = await _usuarioService.ObtenerMiPerfilAsync(userId);
            if (perfil == null) return NotFound("Usuario no encontrado.");

            return Ok(perfil);
        }

        // 2. ACTUALIZAR MI PERFIL
        [HttpPut("perfil")]
        public async Task<IActionResult> UpdateMiPerfil([FromBody] UsuarioUpdateDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var actualizado = await _usuarioService.ActualizarPerfilAsync(userId, dto);
            if (!actualizado) return BadRequest("No se pudo actualizar el perfil.");

            return Ok(new { mensaje = "Perfil actualizado correctamente" });
        }

        // 3. BUSCAR AMIGO POR CÓDIGO (Ej: /api/usuarios/buscar/DANI92)
        [HttpGet("buscar/{codigoAmigo}")]
        public async Task<IActionResult> BuscarAmigo(string codigoAmigo)
        {
            if (string.IsNullOrEmpty(codigoAmigo)) return BadRequest("Debes enviar un código.");

            var perfilAmigo = await _usuarioService.BuscarAmigoPorCodigoAsync(codigoAmigo);
            
            if (perfilAmigo == null) 
                return NotFound(new { mensaje = "No se encontró ningún atleta con ese código 😢" });

            return Ok(perfilAmigo);
        }
    }
}