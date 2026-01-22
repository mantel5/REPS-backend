namespace REPS_backend.Models
{
    public class RutinaEjercicio
    {
        public int Id { get; set; }
        public int RutinaId { get; set; }
        public int EjercicioId { get; set; }
        public int Orden { get; set; } 
        
        // Config visual
        public int Series { get; set; } 
        public string Repeticiones { get; set; } = "10-12";
        public int DescansoSegundos { get; set; } = 90;

        // --- SMART WEIGHT ---
        // Si es DropSet, usa el PorcentajeDelPeso para sugerir carga
        public TipoSerie Tipo { get; set; } = TipoSerie.Normal; 
        public decimal PorcentajeDelPeso { get; set; } = 1.0m; 
        
        public double PesoSugerido { get; set; } 
    }
}