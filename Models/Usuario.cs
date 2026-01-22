namespace REPS_backend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        
        public string AvatarId { get; set; } = CatalogoAvatars.Default;
        public string Rol { get; set; } = Models.Rol.User;

        // --- SUSCRIPCIÓN ---
        public PlanSuscripcion PlanActual { get; set; } = PlanSuscripcion.Gratuito;
        public DateTime FechaFinSuscripcion { get; set; } 

        // --- DASHBOARD ---
        public int PuntosTotales { get; set; }
        public int RachaDias { get; set; }
        public string RangoGeneral { get; set; } = "Bronce";

        // --- MÉTODOS ---
        public void SetPassword(string password) => 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password) => 
            BCrypt.Net.BCrypt.Verify(password, PasswordHash);

        public bool EsPro() => 
            PlanActual == PlanSuscripcion.ProMensual && FechaFinSuscripcion > DateTime.Now;
    }
}