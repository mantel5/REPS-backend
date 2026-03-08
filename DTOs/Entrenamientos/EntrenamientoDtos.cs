namespace REPS_backend.DTOs.Entrenamientos
{
    public class FinalizarEntrenamientoDto
    {
        public int? RutinaId { get; set; }
        public string Nombre { get; set; } = "Entrenamiento sin nombre";
        public int DuracionMinutos { get; set; }
        public List<EjercicioRealizadoDto> Ejercicios { get; set; } = new();
    }

    public class EjercicioRealizadoDto
    {
        public int EjercicioId { get; set; }
        // El peso máximo se calculará en el servidor buscando entre las series
        public List<SerieDto> Series { get; set; } = new();
    }
}
