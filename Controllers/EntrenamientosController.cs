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
        private readonly IRankingService _rankingService;

        public EntrenamientosController(IEntrenamientoService entrenamientoService, IRankingService rankingService)
        {
            _entrenamientoService = entrenamientoService;
            _rankingService = rankingService;
        }

        [HttpPost("finalizar")]
        public async Task<IActionResult> FinalizarEntrenamiento([FromBody] FinalizarEntrenamientoDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var resultado = await _entrenamientoService.FinalizarEntrenamientoAsync(userId, dto);

            // Recalcular ranking y racha después de cada entrenamiento
            await _rankingService.UpdateUserRankAsync(userId);
            await _rankingService.UpdateStreakAsync(userId);

            return Ok(resultado);
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
