using REPS_backend.Models;

namespace REPS_backend.DTOs.Ejercicios
{
    public class EjercicioItemDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public GrupoMuscular GrupoMuscular { get; set; }
        public string ImagenMusculosUrl { get; set; }
    }
}