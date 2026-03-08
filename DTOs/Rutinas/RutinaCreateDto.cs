using REPS_backend.Models; // Para acceder a los Enums
using System.Text.Json.Serialization;

namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaCreateDto
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }
        
        [JsonPropertyName("nivel")]
        public NivelDificultad Nivel { get; set; }

        [JsonPropertyName("imagenUrl")]
        public string ImagenUrl { get; set; } = string.Empty;
        // La duración se calcula en el backend, no se recibe.
        // El estado por defecto se maneja en el servicio.
        
        [JsonPropertyName("ejercicios")]
        public List<RutinaEjercicioDto> Ejercicios { get; set; } = new();
    }
}