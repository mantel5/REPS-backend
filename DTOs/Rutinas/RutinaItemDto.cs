namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaItemDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        
        // Devolvemos el nombre del Enum (ej: "Avanzado") para facilitar la vida al Frontend
        public string Nivel { get; set; } 
        
        public int DuracionMinutos { get; set; }
        
        // ponemos el número total para que sepan si es larga o corta.
        public int CantidadEjercicios { get; set; } 
    }
}