namespace REPS_backend.Models
{
    public class Ejercicio
    {
        public int Id { get; set; }
        public int? UsuarioCreadorId { get; set; } 

        public string Nombre { get; set; } = ""; 
        public string GrupoMuscular { get; set; } = "";
        
        public string DescripcionTecnica { get; set; } = ""; 
        
        public string ImagenMusculosUrl { get; set; } = ""; 
        public List<DetalleMuscular> MusculosInvolucrados { get; set; } = new List<DetalleMuscular>();
    }
}
