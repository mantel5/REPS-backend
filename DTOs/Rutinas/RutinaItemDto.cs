namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaItemDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        
        // Devolvemos el nombre del Enum (ej: "Avanzado") para facilitar la vida al Frontend
        public string Nivel { get; set; } 
        
        public int DuracionMinutos { get; set; }
        public string UrlImagen { get; set; } = string.Empty;
        public string CreadorNombre { get; set; } = string.Empty;
        public int Likes { get; set; }
        
        // ponemos el número total para que sepan si es larga o corta.
        public int CantidadEjercicios { get; set; } 
        public int TotalEjercicios { get; set; } 

        public string Estado { get; set; } = string.Empty;
        public List<string> Musculos { get; set; } = new List<string>();
    }
}