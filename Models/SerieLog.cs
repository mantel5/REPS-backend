namespace REPS_backend.Models
{
    public class SerieLog
    {
        public int Id { get; set; }
        public int SesionId { get; set; }
        public int EjercicioId { get; set; } 
        
        public int NumeroSerie { get; set; } 
        public decimal PesoUsado { get; set; }
        public int RepsRealizadas { get; set; }
        
        public bool Completada { get; set; } = true; 
    }
}
