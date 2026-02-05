namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public List<int> EjerciciosIds { get; set; } = new List<int>();
    }
}
