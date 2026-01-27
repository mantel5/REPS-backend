namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaDetalleDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CreadorNombre { get; set; } = string.Empty;
        public List<EjercicioEnRutinaDto> Ejercicios { get; set; } = new List<EjercicioEnRutinaDto>();
    }
    public class EjercicioEnRutinaDto
    {
        public int EjercicioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Series { get; set; }
        public string Repeticiones { get; set; } = string.Empty; 
    }
}
