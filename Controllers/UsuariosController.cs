using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REPS_backend.DTOs.Usuarios;
using REPS_backend.Services;
using REPS_backend.Models; 
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


        [HttpGet("perfil")]
        public async Task<IActionResult> GetMiPerfil()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var perfil = await _usuarioService.ObtenerMiPerfilAsync(userId);
            if (perfil == null) return NotFound("Usuario no encontrado.");

            return Ok(perfil);
        }

        [HttpPut("perfil")]
        public async Task<IActionResult> UpdateMiPerfil([FromBody] UsuarioUpdateDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var actualizado = await _usuarioService.ActualizarPerfilAsync(userId, dto);
            if (!actualizado) return BadRequest("No se pudo actualizar el perfil.");

            return Ok(new { mensaje = "Perfil actualizado correctamente" });
        }

        [HttpGet("buscar/{codigoAmigo}")]
        public async Task<IActionResult> BuscarAmigo(string codigoAmigo)
        {
            if (string.IsNullOrEmpty(codigoAmigo)) return BadRequest("Debes enviar un código.");

            var perfilAmigo = await _usuarioService.BuscarAmigoPorCodigoAsync(codigoAmigo);
            
            if (perfilAmigo == null) 
                return NotFound(new { mensaje = "No se encontró ningún atleta con ese código 😢" });

            return Ok(perfilAmigo);
        }

        // ... dentro de UsuariosController

        // 4. AGREGAR AMIGO (POST)
        [HttpPost("amigos/agregar/{codigoAmigo}")]
        public async Task<IActionResult> AgregarAmigo(string codigoAmigo)
        {
            // 1. Sacamos mi ID del token
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            if (string.IsNullOrEmpty(codigoAmigo)) return BadRequest("Falta el código.");

            // 2. Llamamos al servicio
            var exito = await _usuarioService.AgregarAmigoAsync(userId, codigoAmigo);

            if (!exito)
            {
                return BadRequest(new { mensaje = "No se pudo agregar. Verifica que el código existe, que no eres tú mismo y que no sois ya amigos." });
            }

            return Ok(new { mensaje = "¡Amigo agregado correctamente! 🤝" });
        }

        [HttpGet("admin/todos")]
        [Authorize(Roles = Rol.Admin)] 
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _usuarioService.ObtenerTodosLosUsuariosAdminAsync();
            return Ok(usuarios);
        }
        [HttpPut("admin/estado/{id}")]
        [Authorize(Roles = Rol.Admin)]
        public async Task<IActionResult> CambiarEstadoUsuario(int id, [FromBody] bool nuevoEstado)
        {
            var exito = await _usuarioService.CambiarEstadoBloqueoAsync(id, nuevoEstado);
            if (!exito) return NotFound("Usuario no encontrado");

            return Ok(new { mensaje = $"Estado del usuario actualizado a: {(nuevoEstado ? "ACTIVO" : "BLOQUEADO")}" });
        }

        [HttpDelete("admin/eliminar/{id}")]
        [Authorize(Roles = Rol.Admin)]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var exito = await _usuarioService.EliminarUsuarioLogicoAsync(id);
            if (!exito) return NotFound("Usuario no encontrado");

            return Ok(new { mensaje = "Usuario eliminado correctamente (Baja Lógica)." });
        }
    }
}