namespace REPS_backend.Models
{
    public class Sesion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        
        public string NombreRutinaSnapshot { get; set; } = "";
        public DateTime Fecha { get; set; }
        
        public bool EstaFinalizada { get; set; } = false; 
        public int DuracionRealMinutos { get; set; }
        public int PuntosGanados { get; set; }

        public List<SerieLog> SeriesRealizadas { get; set; } = new List<SerieLog>();
    }
}
