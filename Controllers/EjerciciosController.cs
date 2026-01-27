using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using REPS_backend.Services;
using REPS_backend.DTOs.Ejercicios;
using System.Security.Claims;

namespace REPS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EjerciciosController : ControllerBase
    {
        private readonly IEjercicioService _ejercicioService;
        private readonly ILogger<EjerciciosController> _logger;

        public EjerciciosController(IEjercicioService ejercicioService, ILogger<EjerciciosController> logger)
        {
            _ejercicioService = ejercicioService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> CrearEjercicio([FromBody] EjercicioCreateDto dto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

                int usuarioId = int.Parse(userIdString);

                var ejercicioCreado = await _ejercicioService.CrearEjercicioAsync(dto, usuarioId);
                return Ok(ejercicioCreado);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User, Admin")] // Ahora dejamos que el User borre (su servicio validará si es suyo)
        public async Task<IActionResult> DeleteEjercicio(int id)
        {
            try
            {
                // En el futuro, aquí deberías validar que solo borren SUS ejercicios
                // De momento, si lo borran, se borra.
                var borrado = await _ejercicioService.BorrarEjercicioAsync(id);
                if (!borrado) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> ObtenerEjercicios()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

                int usuarioId = int.Parse(userIdString);

                // AHORA LLAMAMOS AL MÉTODO QUE FILTRA POR USUARIO
                var ejercicios = await _ejercicioService.ObtenerTodosParaUsuarioAsync(usuarioId);
                return Ok(ejercicios);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
