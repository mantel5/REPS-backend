namespace RepsAPI.Models
{
    public enum NivelRutina
    {
        Principiante,
        Intermedio,
        Avanzado
    }

    public enum VisibilidadRutina
    {
        Privada,
        Publica
    }

    public enum EstadoRutina
    {
        EnEspera,
        Publicada,
        Rechazada,
        Bloqueada
    }

    public class Rutina
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public NivelRutina Nivel { get; set; }
        public VisibilidadRutina Visibilidad { get; set; }
        public EstadoRutina Estado { get; set; }
        
        public string? MotivoRechazo { get; set; }

        public List<RutinaEjercicio> RutinaEjercicios { get; set; } = new List<RutinaEjercicio>();
    }
}