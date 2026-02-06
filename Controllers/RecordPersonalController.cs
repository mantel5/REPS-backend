using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REPS_backend.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace REPS_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordPersonalController : ControllerBase
    {
        private readonly IRecordPersonalService _recordService;

        public RecordPersonalController(IRecordPersonalService recordService)
        {
            _recordService = recordService;
        }

        [HttpPost("registrar")]
        // [Authorize] // Descomentar cuando la auth esté lista y se requiera
        public async Task<IActionResult> RegistrarLevantamiento([FromQuery] int ejercicioId, [FromQuery] double peso)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized("Usuario no identificado");

            var esRecord = await _recordService.RegistrarNuevoLevantamientoAsync(userId, ejercicioId, peso);

            if (esRecord)
            {
                return Ok(new { Message = "¡Nuevo Record Personal! Puntos añadidos.", EsRecord = true });
            }
            
            return Ok(new { Message = "Levantamiento registrado.", EsRecord = false });
        }

        private int GetCurrentUserId()
        {
            // TODO: Implementar lógica real de Claims cuando haya Auth
            // Por ahora, para pruebas, retornamos 1 o buscamos el claim
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }
            // Fallback para desarrollo/pruebas si no hay token
            return 1; 
        }
    }
}
