namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaEjercicioUpdateDto
    {
        public int Series { get; set; }
        public string Repeticiones { get; set; } = string.Empty;
        public int DescansoSegundos { get; set; }
        public decimal PorcentajeDelPeso { get; set; }
        public double PesoSugerido { get; set; }
    }
}
