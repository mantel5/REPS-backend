using REPS_backend.Models; // Para acceder a los Enums

namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaCreateDto
    {
        public string Nombre { get; set; }
        public NivelDificultad Nivel { get; set; }
        // La duración se calcula en el backend, no se recibe.
        // El estado por defecto se maneja en el servicio.
        
        public List<RutinaEjercicioDto> Ejercicios { get; set; } = new();
    }
}