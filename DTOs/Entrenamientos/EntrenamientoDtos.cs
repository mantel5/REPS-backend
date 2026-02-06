namespace REPS_backend.DTOs.Entrenamientos
{
    public class FinalizarEntrenamientoDto
    {
        public string Nombre { get; set; } = "Entrenamiento sin nombre";
        public int DuracionMinutos { get; set; }
        public List<EjercicioRealizadoDto> Ejercicios { get; set; } = new();
    }

    public class EjercicioRealizadoDto
    {
        public int EjercicioId { get; set; }
        public double PesoMaximo { get; set; }
    }
}
