using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using REPS_backend.Services;
using REPS_backend.DTOs.Rutinas;
using System.Security.Claims;

namespace REPS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RutinaEjerciciosController : ControllerBase
    {
        private readonly IRutinaEjercicioService _service;
        private readonly ILogger<RutinaEjerciciosController> _logger;

        public RutinaEjerciciosController(IRutinaEjercicioService service, ILogger<RutinaEjerciciosController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> AgregarEjercicio([FromBody] RutinaEjercicioAddDto dto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                int usuarioId = int.Parse(userIdString);

                var resultado = await _service.AgregarEjercicioARutinaAsync(dto, usuarioId);
                if (!resultado) return BadRequest("No se pudo agregar el ejercicio o no tienes permiso.");

                return Ok("Ejercicio agregado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> ActualizarEjercicio(int id, [FromBody] RutinaEjercicioUpdateDto dto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                int usuarioId = int.Parse(userIdString);

                var resultado = await _service.ActualizarRutinaEjercicioAsync(id, dto, usuarioId);
                if (!resultado) return NotFound("No encontrado o sin permiso.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> EliminarEjercicio(int id)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                int usuarioId = int.Parse(userIdString);

                var resultado = await _service.EliminarEjercicioDeRutinaAsync(id, usuarioId);
                if (!resultado) return NotFound("No encontrado o sin permiso.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
