using System.ComponentModel.DataAnnotations.Schema;
namespace REPS_backend.Models
{
    public class Rutina
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string ImagenUrl { get; set; } = ""; 
        public NivelDificultad Nivel { get; set; } = NivelDificultad.Intermedio; 
        public int DuracionMinutos { get; set; } 
        public bool EsPublica { get; set; } 
        public EstadoRutina Estado { get; set; } = EstadoRutina.Privada;
        public bool EsGeneradaPorIA { get; set; } 
        public int Likes { get; set; } 
        public int Descargas { get; set; }
        public List<RutinaEjercicio> Ejercicios { get; set; } = new List<RutinaEjercicio>();
        public void CalcularDuracionEstimada()
        {
            if (Ejercicios == null || Ejercicios.Count == 0) { DuracionMinutos = 0; return; }
            double segundosTotales = 0;
            foreach (var ejer in Ejercicios)
            {
                int tiempoPorSerie = ejer.DescansoSegundos + 60; 
                segundosTotales += (ejer.Series * tiempoPorSerie);
            }
            DuracionMinutos = (int)Math.Ceiling(segundosTotales / 60);
        }
    }
}
