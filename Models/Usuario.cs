using System.ComponentModel.DataAnnotations.Schema;

namespace REPS_backend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string CodigoAmigo { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public string AvatarId { get; set; } = "default"; // He puesto un string simple por si no tienes la clase CatalogoAvatars a mano
        public string? AvatarUrl { get; set; } // Propiedad para la imagen subida a Cloudinary
        public string Rol { get; set; } = "User"; // Igual, simplificado por seguridad

        public PlanSuscripcion PlanActual { get; set; } = PlanSuscripcion.Gratuito;
        public DateTime FechaFinSuscripcion { get; set; }
        public bool EstaActivo { get; set; } = true;
        public bool EstaBorrado { get; set; } = false;
        public UnidadPeso UnidadPreferida { get; set; } = UnidadPeso.Kg;

        public int PuntosLogros { get; set; }
        public int PuntosRecords { get; set; }

        // PuntosTotales es la suma de todo (se puede calcular o persistir, aquí persistimos para facilitar queries)
        public int PuntosTotales { get; set; }

        public int RachaDias { get; set; }
        public DateTime? FechaUltimaActividad { get; set; }
        public Rango RangoGeneral { get; set; } = Rango.Bronce;


        public void SetPassword(string password) =>
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password) =>
            BCrypt.Net.BCrypt.Verify(password, PasswordHash);

        public bool EsPro() =>
            PlanActual == PlanSuscripcion.ProMensual && FechaFinSuscripcion > DateTime.Now;
    }
}