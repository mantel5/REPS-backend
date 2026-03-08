using REPS_backend.Models; // Necesario para ver el Enum

namespace REPS_backend.DTOs.Ejercicios
{
    public class EjercicioCreateDto
    {
        public string Nombre { get; set; }
        public GrupoMuscular GrupoMuscular { get; set; } 
        
        public string? DescripcionTecnica { get; set; }
        public string? ImagenMusculosUrl { get; set; }
    }
}