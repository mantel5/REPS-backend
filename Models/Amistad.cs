using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REPS_backend.Models
{
    public class Amistad
    {

        public int SolicitanteId { get; set; }
        public Usuario Solicitante { get; set; }

        public int ReceptorId { get; set; }
        public Usuario Receptor { get; set; }

        public DateTime FechaAmistad { get; set; } = DateTime.UtcNow;
        
        public bool Aceptada { get; set; } = true; 
    }
}