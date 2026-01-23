using REPS_backend.Models;

namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaEjercicioDto
    {
        public int EjercicioId { get; set; }
        public int Series { get; set; }
        public int DescansoSegundos { get; set; }
        public TipoSerie Tipo { get; set; }
        // Smart Weight se calcula en backend, no es necesario pedirlo aquí
    }
}