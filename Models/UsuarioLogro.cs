namespace REPS_backend.Models
{
    public class UsuarioLogro
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int LogroId { get; set; }
        
        public double Progreso { get; set; } 
        public bool Desbloqueado { get; set; } 
    }
}
