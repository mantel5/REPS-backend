namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaEjercicioAddDto
    {
        public int RutinaId { get; set; }
        public int EjercicioId { get; set; }
        public int Series { get; set; }
        public string Repeticiones { get; set; } = string.Empty;
    }
}
