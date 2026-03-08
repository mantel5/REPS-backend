namespace REPS_backend.Models
{
    public class SerieLog
    {
        public int Id { get; set; }
        public int? SesionId { get; set; }
        public int EjercicioId { get; set; } 
        public virtual Ejercicio Ejercicio { get; set; } = null!;
        
        public int NumeroSerie { get; set; } 
        public decimal PesoUsado { get; set; }
        public decimal Peso => PesoUsado; 
        public int RepsRealizadas { get; set; }
        public int Repeticiones => RepsRealizadas; // Alias for compatibility
        
        public bool Completada { get; set; } = true; 
    }
}
