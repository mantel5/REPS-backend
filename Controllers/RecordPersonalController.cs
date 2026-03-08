using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REPS_backend.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace REPS_backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RecordPersonalController : ControllerBase
    {
        private readonly IRecordPersonalService _recordService;

        public RecordPersonalController(IRecordPersonalService recordService)
        {
            _recordService = recordService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarLevantamiento([FromQuery] int ejercicioId, [FromQuery] double peso)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized("Usuario no identificado");

            var esRecord = await _recordService.RegistrarNuevoLevantamientoAsync(userId, ejercicioId, (decimal)peso);

            if (esRecord)
            {
                return Ok(new { Message = "¡Nuevo Record Personal! Puntos añadidos.", EsRecord = true });
            }

            return Ok(new { Message = "Levantamiento registrado.", EsRecord = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetMisRecords()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized("Usuario no identificado");

            try 
            {
                var records = await _recordService.ObtenerRecordsUsuarioAsync(userId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }
            return 0; // Cambiado para no devolver 1 por defecto en prod
        }
    }
}
