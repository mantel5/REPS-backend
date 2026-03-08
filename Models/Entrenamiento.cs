using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REPS_backend.Models
{
    public class Entrenamiento
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int? RutinaId { get; set; }
        [ForeignKey("RutinaId")]
        public Rutina? Rutina { get; set; }

        public string Nombre { get; set; } = string.Empty; // Ej. "Rutina de Pecho"
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public int DuracionMinutos { get; set; }

        public List<SerieLog> SeriesRealizadas { get; set; } = new();
    }
}
