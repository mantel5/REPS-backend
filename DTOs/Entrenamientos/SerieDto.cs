namespace REPS_backend.DTOs.Entrenamientos
{
    public class SerieDto
    {
        public int NumeroSerie { get; set; }
        public decimal Peso { get; set; }
        public int Reps { get; set; }
        public bool Completada { get; set; } = true;
    }
}
