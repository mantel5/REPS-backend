using Microsoft.AspNetCore.Mvc;
using REPS_backend.DTOs.Rutinas;
using REPS_backend.Services;

namespace REPS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class RutinasController : ControllerBase
    {
        private readonly IRutinaService _rutinaService;
        private readonly ILogger<RutinasController> _logger;

        public RutinasController(IRutinaService rutinaService, ILogger<RutinasController> logger)
        {
            _rutinaService = rutinaService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [HttpPost]
        public async Task<ActionResult<RutinaDetalleDto>> CrearRutina([FromBody] RutinaCreateDto dto)
        {
            _logger.LogInformation($"Creando rutina: {dto.Nombre}. Cantidad ejercicios DTO: {dto.Ejercicios?.Count ?? 0}");
            if (dto.Ejercicios != null)
            {
                foreach (var e in dto.Ejercicios)
                {
                    _logger.LogInformation($"Ejercicio ID: {e.EjercicioId}, Tipo: {e.Tipo}, Repeticiones: {e.Repeticiones}");
                }
            }
            int usuarioId = GetUserId();
            var rutinaCreada = await _rutinaService.CrearRutinaAsync(dto, usuarioId);
            return CreatedAtAction(nameof(GetRutinaById), new { id = rutinaCreada.Id }, rutinaCreada);
        }

        [HttpGet]
        public async Task<ActionResult<List<RutinaItemDto>>> GetMisRutinas()
        {
            int usuarioId = GetUserId();
            var rutinas = await _rutinaService.ObtenerRutinasUsuarioAsync(usuarioId);
            return Ok(rutinas);
        }

        [HttpGet("comunidad")]
        public async Task<ActionResult<List<RutinaItemDto>>> GetRutinasPublicas()
        {
            var rutinas = await _rutinaService.ObtenerRutinasPublicasAsync();
            return Ok(rutinas);
        }

        // GET: api/rutinas/5
        // (Devuelve el detalle completo "Estilo Netflix Película")
        [HttpGet("{id}")]
        public async Task<ActionResult<RutinaDetalleDto>> GetRutinaById(int id)
        {
            var rutina = await _rutinaService.ObtenerDetalleRutinaAsync(id);

            if (rutina == null)
            {
                return NotFound($"No se encontró ninguna rutina con el ID {id}");
            }

            return Ok(rutina);
        }

        [HttpPost("generar-ia")]
        public async Task<ActionResult<RutinaDetalleDto>> GenerarConIA([FromBody] RutinaIARequestDto dto)
        {
            try
            {
                int usuarioId = GetUserId();
                var rutina = await _rutinaService.GenerarRutinaIAAsync(dto, usuarioId);
                return Ok(rutina);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR CRITICO EN IA]: {ex.ToString()}\n");
                return StatusCode(500, new { error = "AI generation failed", exception = ex.Message });
            }
        }

        [HttpPut("{id}/publicar")]
        public async Task<IActionResult> PublicarRutina(int id)
        {
            try
            {
                int usuarioId = GetUserId();
                var resultado = await _rutinaService.PublicarRutinaAsync(id, usuarioId);
                if (!resultado) return NotFound($"Rutina con ID {id} no encontrada o no se pudo publicar.");
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarRutina(int id)
        {
            try
            {
                int usuarioId = GetUserId();
                var resultado = await _rutinaService.EliminarRutinaAsync(id, usuarioId);
                if (!resultado) return NotFound($"Rutina con ID {id} no encontrada");
                return NoContent();
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("admin/pendientes")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<RutinaItemDto>>> GetRutinasEnRevision()
        {
            var rutinas = await _rutinaService.ObtenerRutinasEnRevisionAsync();
            return Ok(rutinas);
        }

        [HttpGet("admin/todas")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<RutinaItemDto>>> GetTodasRutinasAdmin()
        {
            var rutinas = await _rutinaService.ObtenerTodasRutinasAdminAsync();
            return Ok(rutinas);
        }

        [HttpPut("admin/{id}/validar")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> ValidarRutinaAdmin(int id)
        {
            var resultado = await _rutinaService.ValidarRutinaAsync(id);
            if (!resultado) return NotFound($"Rutina con ID {id} no encontrada.");
            return NoContent();
        }

        [HttpPut("admin/{id}/rechazar")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> RechazarRutinaAdmin(int id)
        {
            var resultado = await _rutinaService.RechazarRutinaAsync(id);
            if (!resultado) return NotFound($"Rutina con ID {id} no encontrada.");
            return NoContent();
        }

        [HttpDelete("admin/{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarRutinaAdmin(int id)
        {
            var resultado = await _rutinaService.EliminarRutinaAdminAsync(id);
            if (!resultado) return NotFound($"Rutina con ID {id} no encontrada.");
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RutinaDetalleDto>> ActualizarRutina(int id, [FromBody] RutinaCreateDto dto)
        {
            try
            {
                int usuarioId = GetUserId();
                var rutinaActualizada = await _rutinaService.ActualizarRutinaAsync(id, dto, usuarioId);
                return Ok(rutinaActualizada);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception e)
            {
                if (e.Message == "Rutina no encontrada.") return NotFound(e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}/like")]
        public async Task<ActionResult<int>> LikeRutina(int id)
        {
            var likes = await _rutinaService.LikeRutinaAsync(id);
            return Ok(likes);
        }

        [HttpPost("{id}/copiar")]
        public async Task<ActionResult<RutinaDetalleDto>> CopiarRutina(int id)
        {
            int usuarioId = GetUserId();
            var copia = await _rutinaService.CopiarRutinaAsync(id, usuarioId);
            return Ok(copia);
        }
    }
}