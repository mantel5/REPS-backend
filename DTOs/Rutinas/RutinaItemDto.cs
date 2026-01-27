namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaItemDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CreadorNombre { get; set; } = string.Empty;
        public int TotalEjercicios { get; set; }
    }
}
