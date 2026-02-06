using Microsoft.AspNetCore.Mvc;
using REPS_backend.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace REPS_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var leaderboard = await _rankingService.GetLeaderboardAsync();
            return Ok(leaderboard);
        }

        [HttpGet("mi-progreso")]
        public async Task<IActionResult> GetMiProgreso()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var progress = await _rankingService.GetUserProgressAsync(userId);
            if (progress == null) return NotFound();

            return Ok(progress);
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }
            return 1; // Fallback
        }
    }
}
