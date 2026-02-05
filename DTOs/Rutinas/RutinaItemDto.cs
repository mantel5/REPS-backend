namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaItemDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string CreadorNombre { get; set; } = "";
        public int TotalEjercicios { get; set; }
        public string ImagenUrl { get; set; } = "";
        public int Likes { get; set; } 
    }
}