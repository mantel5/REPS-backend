using REPS_backend.Models; // Asegúrate de que detecta tus Enums

namespace REPS_backend.Models
{
    public class Ejercicio
    {
        public int Id { get; set; }
        public int? UsuarioCreadorId { get; set; } 

        public string Nombre { get; set; } = ""; 
        
        public GrupoMuscular GrupoMuscular { get; set; } 
        
        public string DescripcionTecnica { get; set; } = ""; 
        
        public string ImagenMusculosUrl { get; set; } = ""; 
    }
}