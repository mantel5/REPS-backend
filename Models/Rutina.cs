namespace REPS_backend.Models
{
    public class Rutina
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string ImagenUrl { get; set; } = ""; 
        
        public string Nivel { get; set; } = "Intermedio"; 
        public int DuracionMinutos { get; set; } 

        // --- PUBLICACIÓN ---
        public bool EsPublica { get; set; } 
        public EstadoRutina Estado { get; set; } = EstadoRutina.Privada;

        // Stats
        public bool EsGeneradaPorIA { get; set; } 
        public int Likes { get; set; } 
        public int Descargas { get; set; }

        public List<RutinaEjercicio> Ejercicios { get; set; } = new List<RutinaEjercicio>();
    }
}