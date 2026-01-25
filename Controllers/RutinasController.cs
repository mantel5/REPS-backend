using Microsoft.AspNetCore.Mvc;
using REPS_backend.DTOs.Rutinas;
using REPS_backend.Services;

namespace REPS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RutinasController : ControllerBase
    {
        private readonly IRutinaService _rutinaService;

        // Inyectamos el SERVICIO, no el repositorio ni el DbContext
        public RutinasController(IRutinaService rutinaService)
        {
            _rutinaService = rutinaService;
        }

        // POST: api/rutinas
        [HttpPost]
        public async Task<ActionResult<RutinaDetalleDto>> CrearRutina([FromBody] RutinaCreateDto dto)
        {
            // Simulamos un usuario ID 1 (Cuando tengas login real, esto vendrá del token)
            int usuarioId = 1; 

            var rutinaCreada = await _rutinaService.CrearRutinaAsync(dto, usuarioId);

            // Devolvemos 201 Created y los datos de la rutina creada
            return CreatedAtAction(nameof(GetRutinaById), new { id = rutinaCreada.Id }, rutinaCreada);
        }

        // GET: api/rutinas
        // (Devuelve la lista ligera "Estilo Netflix Menú")
        [HttpGet]
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
    }
}