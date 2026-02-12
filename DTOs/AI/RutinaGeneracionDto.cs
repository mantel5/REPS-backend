using System.ComponentModel.DataAnnotations;

namespace REPS_backend.DTOs.AI
{
    public class RutinaGeneracionDto
    {
        [Required]
        public string Objetivo { get; set; } = string.Empty; // e.g., "Ganar masa muscular", "Perder peso"

        [Required]
        public List<string> GruposMusculares { get; set; } = new List<string>(); // e.g., ["Pecho", "Espalda"]

        [Required]
        public int DuracionMinutos { get; set; } // e.g., 60

        [Required]
        public string NivelExperiencia { get; set; } = string.Empty; // e.g., "Intermedio"

        public string? InstruccionesAdicionales { get; set; }
    }
}
