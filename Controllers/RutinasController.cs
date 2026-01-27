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
    public class RutinasController : ControllerBase
    {
        private readonly IRutinaService _rutinaService;
        private readonly ILogger<RutinasController> _logger;

        public RutinasController(IRutinaService rutinaService, ILogger<RutinasController> logger)
        {
            _rutinaService = rutinaService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> CrearRutina([FromBody] RutinaCreateDto dto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int usuarioId = int.Parse(userIdString);

                var rutinaCreada = await _rutinaService.CrearRutinaAsync(dto, usuarioId);
                
                return CreatedAtAction(nameof(GetRutinaById), new { id = rutinaCreada.Id }, rutinaCreada);
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

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetRutinaById(int id)
        {
            try
            {
                var rutina = await _rutinaService.ObtenerDetalleRutinaAsync(id);

                if (rutina == null) return NotFound($"No se encontró ninguna rutina con el ID {id}");

                return Ok(rutina);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}