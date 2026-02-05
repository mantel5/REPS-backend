using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REPS_backend.DTOs.Logros;
using REPS_backend.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace REPS_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogroController : ControllerBase
    {
        private readonly ILogroService _logroService;

        public LogroController(ILogroService logroService)
        {
            _logroService = logroService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetLogrosForUser(int userId)
        {
            var logros = await _logroService.GetLogrosForUserAsync(userId);
            return Ok(logros);
        }

        [HttpGet("recent/{userId}")]
        public async Task<IActionResult> GetRecentLogrosForUser(int userId)
        {
            var logros = await _logroService.GetLogrosForUserAsync(userId);
            var recent = logros
                .Where(l => l.Desbloqueado && l.FechaObtencion.HasValue)
                .OrderByDescending(l => l.FechaObtencion)
                .Take(3)
                .ToList();
            return Ok(recent);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLogro([FromBody] CreateLogroDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _logroService.CreateLogroAsync(dto);
            return CreatedAtAction(nameof(GetLogrosForUser), new { userId = 0 }, created); // userId 0 is placeholder
        }

        // ayuda para obtener el id del usuario actual
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
