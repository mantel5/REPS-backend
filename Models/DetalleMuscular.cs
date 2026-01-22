namespace REPS_backend.Models
{
    public class DetalleMuscular
    {
        public int Id { get; set; }
        public int EjercicioId { get; set; } 

        public string NombreMusculo { get; set; } = ""; 
        public string DescripcionImpacto { get; set; } = ""; 
        public bool EsPrincipal { get; set; } 
    }
}
