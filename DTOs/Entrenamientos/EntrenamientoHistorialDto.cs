using REPS_backend.DTOs.Ejercicios;

namespace REPS_backend.DTOs.Entrenamientos
{
    public class EntrenamientoHistorialDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int DuracionMinutos { get; set; }
        public List<EjercicioHistorialDto> Ejercicios { get; set; } = new();
    }

    public class EjercicioHistorialDto
    {
        public int EjercicioId { get; set; }
        public string NombreEjercicio { get; set; } = string.Empty;
        public List<SerieDto> Series { get; set; } = new();
    }
}
