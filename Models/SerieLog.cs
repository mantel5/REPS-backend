namespace REPS_backend.Models
{
    public class SerieLog
    {
        public int Id { get; set; }
        public int EntrenamientoId { get; set; } // Cambiado de SesionId
        [System.Text.Json.Serialization.JsonIgnore]
        public Entrenamiento? Entrenamiento { get; set; }

        public int EjercicioId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Ejercicio? Ejercicio { get; set; }

        public int NumeroSerie { get; set; }
        public decimal PesoUsado { get; set; }
        public int RepsRealizadas { get; set; }

        public bool Completada { get; set; } = true;

        // Campos para Cardio (valores reales realizados en esta serie/intervalo)
        public decimal? VelocidadReal { get; set; }
        public int? TiempoSegundosReal { get; set; }
        public decimal? InclinacionReal { get; set; }
        public int? CaloriasQuemadasReales { get; set; }
        public decimal? DistanciaReal { get; set; }
    }
}
