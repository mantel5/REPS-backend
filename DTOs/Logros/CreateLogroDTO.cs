using System.ComponentModel.DataAnnotations;

namespace REPS_backend.DTOs.Logros
{
    public class CreateLogroDTO
    {
        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public int Puntos { get; set; }

        public string IconoUrl { get; set; } = "default_trophy.png";
    }
}
