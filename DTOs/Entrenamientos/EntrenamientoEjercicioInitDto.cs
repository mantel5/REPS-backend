using REPS_backend.Models;

namespace REPS_backend.DTOs.Entrenamientos
{
    public class EntrenamientoEjercicioInitDto
    {
        public int EjercicioId { get; set; }
        public string NombreEjercicio { get; set; } = string.Empty;
        public string ImagenMusculosUrl { get; set; } = string.Empty;
        public int SeriesObjetivo { get; set; }
        public string RepeticionesObjetivo { get; set; } = string.Empty;
        public int DescansoSegundos { get; set; }
        public double PesoSugerido { get; set; }
        public TipoSerie Tipo { get; set; }

        public bool EsCardio { get; set; }
        public decimal? VelocidadSugerida { get; set; }
        public int? TiempoSegundosSugerido { get; set; }
        public decimal? InclinacionSugerida { get; set; }
    }
}
