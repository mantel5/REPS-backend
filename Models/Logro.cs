namespace REPS_backend.Models
{
    public class Logro
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Puntos { get; set; }
        public string IconoUrl { get; set; } = "";
    }
}