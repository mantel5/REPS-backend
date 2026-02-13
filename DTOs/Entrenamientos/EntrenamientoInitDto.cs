using REPS_backend.DTOs.Rutinas;

namespace REPS_backend.DTOs.Entrenamientos
{
    public class EntrenamientoInitDto
    {
        public int RutinaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int DuracionEstimadaMinutos { get; set; }
        public List<EntrenamientoEjercicioInitDto> Ejercicios { get; set; } = new();
    }
}
