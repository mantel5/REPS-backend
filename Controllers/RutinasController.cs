using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using REPS_backend.Services;
using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;
using System.Security.Claims;
using REPS_backend.DTOs.AI;

namespace REPS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RutinasController : ControllerBase
    {
        private readonly IRutinaService _rutinaService;
        private readonly ILogger<RutinasController> _logger;
        private readonly REPS_backend.Services.AI.IAIService _aiService;

        public RutinasController(IRutinaService rutinaService, ILogger<RutinasController> logger, REPS_backend.Services.AI.IAIService aiService)
        {
            _rutinaService = rutinaService;
            _logger = logger;
            _aiService = aiService;
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> CrearRutina([FromBody] RutinaCreateDto dto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                int usuarioId = int.Parse(userIdString);

                var rutinaCreada = await _rutinaService.CrearRutinaAsync(dto, usuarioId);
                return CreatedAtAction(nameof(GetRutinaById), new { id = rutinaCreada.Id }, rutinaCreada);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var mensajeError = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                return BadRequest(mensajeError);
            }
        }

        // POST: api/rutinas/generar-ia
        [HttpPost("generar-ia")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<RutinaDetalleDto>> GenerarRutinaIA([FromBody] RutinaGeneracionDto dto)
        {
            try
            {
                var rutinaGenerada = await _aiService.GenerateRoutineAsync(dto);
                return Ok(rutinaGenerada);
            }
            catch (Exception ex)
            {
                return BadRequest("Error generando rutina: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> ActualizarRutina(int id, [FromBody] RutinaUpdateDto dto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                int usuarioId = int.Parse(userIdString);

                var actualizado = await _rutinaService.ActualizarRutinaAsync(id, dto, usuarioId);
                if (!actualizado) return NotFound("Rutina no encontrada o no eres el dueño.");

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
        public async Task<IActionResult> DeleteRutina(int id)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                int usuarioId = int.Parse(userIdString);

                var borrado = await _rutinaService.BorrarRutinaAsync(id, usuarioId);
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
        public async Task<IActionResult> GetRutinasPublicas()
        {
            try
            {
                var rutinas = await _rutinaService.ObtenerRutinasPublicasAsync();
                return Ok(rutinas);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("mis-rutinas")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetMisRutinas([FromQuery] NivelDificultad? nivel = null, [FromQuery] GrupoMuscular? musculo = null)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

                var misRutinas = await _rutinaService.ObtenerRutinasDeUsuarioAsync(userId, nivel, musculo);
                return Ok(misRutinas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Error al obtener tus rutinas.");
            }
        }

        [HttpGet("ia")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetRutinasIA()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

                var rutinas = await _rutinaService.ObtenerRutinasIAAsync(userId);
                return Ok(rutinas);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Manejo específico para el error de Pro lanzado en el servicio
                return StatusCode(403, new { mensaje = ex.Message, requierePro = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Error al obtener rutinas de IA.");
            }
        }

        [HttpGet("guardadas")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetRutinasGuardadas()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

                var rutinas = await _rutinaService.ObtenerRutinasGuardadasAsync(userId);
                return Ok(rutinas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Error al obtener rutinas guardadas.");
            }
        }

        [HttpPost("{id}/importar")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> ImportarRutina(int id)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

                var rutinaImportada = await _rutinaService.ImportarRutinaAsync(id, userId);
                if (rutinaImportada == null) return NotFound("Rutina original no encontrada.");

                return CreatedAtAction(nameof(GetRutinaById), new { id = rutinaImportada.Id }, rutinaImportada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Error al importar la rutina.");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetRutinaById(int id)
        {
            try
            {
                var rutina = await _rutinaService.ObtenerDetalleRutinaAsync(id);
                if (rutina == null) return NotFound();
                return Ok(rutina);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //LIKES
        [HttpPost("{id}/like")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> ToggleLike(int id)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

                bool esLike = await _rutinaService.ToggleLikeAsync(id, userId);

                if (esLike)
                {
                    return Ok(new { liked = true, mensaje = "Like añadido" });
                }
                else
                {
                    return Ok(new { liked = false, mensaje = "Like quitado" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
                var mensajeError = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                return BadRequest($"Error likes: {mensajeError}");
            }
        }


        //Moderacion
        [HttpPut("{id}/enviar-revision")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> EnviarRevision(int id)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

                var exito = await _rutinaService.EnviarARevisionAsync(id, userId);

                if (!exito) return BadRequest("No se pudo enviar. Verifica que la rutina es tuya, no está baneada y no está publicada ya.");

                return Ok(new { mensaje = "Rutina enviada a revisión correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("admin/pendientes")]
        [Authorize(Roles = Rol.Admin)]
        public async Task<IActionResult> GetPendientes()
        {
            var pendientes = await _rutinaService.ObtenerRutinasPendientesAsync();
            return Ok(pendientes);
        }

        public class ValidacionDto { public bool Aprobar { get; set; } }

        [HttpPut("admin/validar/{id}")]
        [Authorize(Roles = Rol.Admin)]
        public async Task<IActionResult> ValidarRutina(int id, [FromBody] ValidacionDto dto)
        {
            var exito = await _rutinaService.ValidarRutinaAsync(id, dto.Aprobar);
            if (!exito) return NotFound("Rutina no encontrada.");

            string estado = dto.Aprobar ? "PUBLICADA" : "RECHAZADA";
            return Ok(new { mensaje = $"La rutina ha sido {estado}." });
        }

        [HttpDelete("admin/banear/{id}")]
        [Authorize(Roles = Rol.Admin)]
        public async Task<IActionResult> BanearRutina(int id)
        {
            var exito = await _rutinaService.BanearRutinaAsync(id);
            if (!exito) return NotFound("Rutina no encontrada.");

            return Ok(new { mensaje = "Rutina baneada por infringir las normas." });
        }
    }
}