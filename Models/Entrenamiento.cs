using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REPS_backend.Models
{
    public class Entrenamiento
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public string Nombre { get; set; } = string.Empty; // Ej. "Rutina de Pecho"
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public int DuracionMinutos { get; set; }
        
        // Podríamos guardar un JSON con el detalle o una tabla relacionada EntrenamientoEjercicios
        // Por ahora, para el MVP y el ranking, nos basta saber que existió.
        // Pero el servicio recibirá el detalle para procesar records.
    }
}
