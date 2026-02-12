namespace REPS_backend.DTOs.Entrenamientos
{
    public class EntrenamientoResultadoDto
    {
        public int EntrenamientoId { get; set; }
        public string Mensaje { get; set; } = "Entrenamiento guardado correctamente.";
        public string ConsejoIA { get; set; } = string.Empty;
    }
}
