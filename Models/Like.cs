using System.ComponentModel.DataAnnotations.Schema;

namespace REPS_backend.Models
{
    public class Like
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public int RutinaId { get; set; }
        [ForeignKey("RutinaId")]
        public Rutina? Rutina { get; set; }

        public DateTime FechaLike { get; set; } = DateTime.UtcNow;
    }
}