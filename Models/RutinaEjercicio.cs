namespace REPS_backend.Models
{
    public class RutinaEjercicio
    {
        public int Id { get; set; }
        public int RutinaId { get; set; }
        public int EjercicioId { get; set; }
        public int Orden { get; set; } 
        
        public int Series { get; set; } 
        public string Repeticiones { get; set; } = ""10-12"";
        public int DescansoSegundos { get; set; } = 90;

        public TipoSerie Tipo { get; set; } = TipoSerie.Normal; 
        public decimal PorcentajeDelPeso { get; set; } = 1.0m; 
        
        public double PesoSugerido { get; set; } 
    }
}
