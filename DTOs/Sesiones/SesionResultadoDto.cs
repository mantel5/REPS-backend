namespace REPS_backend.DTOs.Sesiones
{
    public class SesionResultadoDto
    {
        public int SesionId { get; set; }
        public int PuntosGanados { get; set; }
        public string MensajeExito { get; set; } = string.Empty;
        public string ConsejoIA { get; set; } = string.Empty;
    }
}
