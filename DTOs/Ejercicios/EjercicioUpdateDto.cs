using REPS_backend.Models;
namespace REPS_backend.DTOs.Ejercicios
{
    public class EjercicioUpdateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public GrupoMuscular GrupoMuscular { get; set; }
        public string? DescripcionTecnica { get; set; }
        public string? ImagenMusculosUrl { get; set; }
    }
}
