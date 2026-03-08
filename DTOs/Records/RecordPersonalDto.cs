namespace REPS_backend.DTOs.Records
{
    public class RecordPersonalDto
    {
        public int EjercicioId { get; set; }
        public string EjercicioNombre { get; set; } = string.Empty;
        public string GrupoMuscular { get; set; } = string.Empty;
        public decimal PesoMaximo { get; set; }
        public decimal Mejora { get; set; } // PesoMaximo - PesoAnterior
        public DateTime Fecha { get; set; }
        public string TiempoAtras { get; set; } = string.Empty; // "Hace 3 días"
    }
}
