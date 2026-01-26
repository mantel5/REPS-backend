using Microsoft.AspNetCore.Mvc;
using REPS_backend.DTOs.Ejercicios;
using REPS_backend.Services;

namespace REPS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EjerciciosController : ControllerBase
    {
        private readonly IEjercicioService _ejercicioService;

        // Inyectamos el cerebro (Service)
        public EjerciciosController(IEjercicioService ejercicioService)
        {
            _ejercicioService = ejercicioService;
        }

        // GET: api/Ejercicios
        // Para ver la lista de todos los ejercicios
        [HttpGet]
        public async Task<ActionResult<List<EjercicioItemDto>>> ObtenerEjercicios()
        {
            var ejercicios = await _ejercicioService.ObtenerTodosAsync();
            return Ok(ejercicios);
        }

        // POST: api/Ejercicios
        // Para crear uno nuevo (Press Banca, Sentadilla...)
        [HttpPost]
        public async Task<ActionResult<EjercicioItemDto>> CrearEjercicio([FromBody] EjercicioCreateDto dto)
        {
            // Simulamos que lo crea el usuario ID 1 (ya pondremos login real más tarde)
            int usuarioId = 1;

            var ejercicioCreado = await _ejercicioService.CrearEjercicioAsync(dto, usuarioId);
            
            // Devolvemos un código 200 OK con el ejercicio creado
            return Ok(ejercicioCreado);
        }
    }
}