using System.Text.Json.Serialization;

namespace REPS_backend.DTOs.Entrenamientos
{
    /// <summary>
    /// Resultado devuelto al finalizar un entrenamiento.
    /// Incluye los récords batidos y los logros desbloqueados en esta sesión.
    /// </summary>
    public class FinalizarResultadoDto
    {
        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; } = "Entrenamiento guardado.";

        [JsonPropertyName("puntosGanados")]
        public int PuntosGanados { get; set; } = 0;

        [JsonPropertyName("recordsPersonal")]
        public List<RecordEnSesionDto> RecordsPersonal { get; set; } = new();

        [JsonPropertyName("logrosDesbloqueados")]
        public List<LogroEnSesionDto> LogrosDesbloqueados { get; set; } = new();
    }

    public class RecordEnSesionDto
    {
        [JsonPropertyName("ejercicioId")]
        public int EjercicioId { get; set; }

        [JsonPropertyName("ejercicioNombre")]
        public string EjercicioNombre { get; set; } = string.Empty;

        [JsonPropertyName("grupoMuscular")]
        public string GrupoMuscular { get; set; } = string.Empty;

        [JsonPropertyName("pesoMaximo")]
        public decimal PesoMaximo { get; set; }

        [JsonPropertyName("mejora")]
        public decimal Mejora { get; set; }
    }

    public class LogroEnSesionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("puntos")]
        public int Puntos { get; set; }
    }
}
