namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaDetalleDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Nivel { get; set; }
        public int DuracionMinutos { get; set; }
        public string Estado { get; set; } // Aquí quizás interesa saber si está "EnRevision"
        public List<RutinaEjercicioDto> Ejercicios { get; set; }
    }
}