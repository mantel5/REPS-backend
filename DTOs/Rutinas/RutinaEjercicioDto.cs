using REPS_backend.Models;
using System.Text.Json.Serialization;

namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaEjercicioDto
    {
        [JsonPropertyName("ejercicioId")]
        public int EjercicioId { get; set; }
        
        [JsonPropertyName("nombreEjercicio")]
        public string NombreEjercicio { get; set; } = string.Empty;
        
        [JsonPropertyName("grupoMuscular")]
        public string GrupoMuscular { get; set; } = string.Empty;
        
        [JsonPropertyName("series")]
        public int Series { get; set; }
        
        [JsonPropertyName("descansoSegundos")]
        public int DescansoSegundos { get; set; }
        
        [JsonPropertyName("tipo")]
        public TipoSerie Tipo { get; set; }
        
        [JsonPropertyName("repeticiones")]
        public string Repeticiones { get; set; } = string.Empty;

        [JsonPropertyName("ultimoPeso")]
        public decimal UltimoPeso { get; set; } = 0;
    }
}
