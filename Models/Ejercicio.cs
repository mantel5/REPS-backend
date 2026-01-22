namespace REPS_backend.Models
{
    public class Ejercicio
    {
        public int Id { get; set; }
        
        // NULL = Ejercicio del Sistema (público)
        // CON ID = Ejercicio creado por usuario (privado)
        public int? UsuarioCreadorId { get; set; } 

        public string Nombre { get; set; } = ""; 
        public string GrupoMuscular { get; set; } = ""; 
        public string VideoUrl { get; set; } = ""; 
        
        // --- PESTAÑAS DETALLE ---
        public string DescripcionTecnica { get; set; } = ""; 
        public string ImagenMusculosUrl { get; set; } = ""; 
    }
}