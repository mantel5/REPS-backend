namespace REPS_backend.DTOs.Entrenamientos
{
    public class SerieDto
    {
        public int NumeroSerie { get; set; }
        public decimal Peso { get; set; }
        public int Reps { get; set; }
        public bool Completada { get; set; } = true;

        // Campos de Cardio Reales
        public decimal? VelocidadReal { get; set; }
        public int? TiempoSegundosReal { get; set; }
        public decimal? InclinacionReal { get; set; }
        public int? CaloriasQuemadasReales { get; set; }
        public decimal? DistanciaReal { get; set; }
    }
}
