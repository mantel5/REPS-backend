using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REPS_backend.DTOs.Entrenamientos;
using REPS_backend.Services;
using System.Security.Claims;

namespace REPS_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EntrenamientosController : ControllerBase
    {
        private readonly IEntrenamientoService _entrenamientoService;

        public EntrenamientosController(IEntrenamientoService entrenamientoService)
        {
            _entrenamientoService = entrenamientoService;
        }

        [HttpPost("finalizar")]
        public async Task<ActionResult<EntrenamientoResultadoDto>> FinalizarEntrenamiento([FromBody] FinalizarEntrenamientoDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var resultado = await _entrenamientoService.FinalizarEntrenamientoAsync(userId, dto);
            return Ok(resultado);
        }
        [HttpGet("iniciar/{rutinaId}")]
        public async Task<ActionResult<EntrenamientoInitDto>> IniciarEntrenamiento(int rutinaId)
        {
            var initDto = await _entrenamientoService.IniciarEntrenamientoAsync(rutinaId);
            if (initDto == null) return NotFound("Rutina no encontrada");
            return Ok(initDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetHistorial()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var historial = await _entrenamientoService.ObtenerHistorialUsuarioAsync(userId);
            return Ok(historial);
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
