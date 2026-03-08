namespace REPS_backend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        
        public string AvatarId { get; set; } = "";
        public string Rol { get; set; } = Models.Rol.User;

        public PlanSuscripcion PlanActual { get; set; } = PlanSuscripcion.Gratuito;
        public DateTime FechaFinSuscripcion { get; set; } 

        public int PuntosTotales { get; set; } 
        public int PuntosLogros { get; set; } 
        public int PuntosRecords { get; set; } 
        public int RachaDias { get; set; }
        public DateTime? FechaUltimaActividad { get; set; }
        public Rango RangoGeneral { get; set; } = Rango.Bronce;

        public string? Biografia { get; set; } = "Apasionado del fitness y la vida saludable";
        public bool EsPerfilPublico { get; set; } = true;
        public bool MostrarEstadisticas { get; set; } = true;
        public bool RankingVisible { get; set; } = true;

        public string CodigoAmigo { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public bool EstaActivo { get; set; } = true;
        public bool EstaBorrado { get; set; } = false;

        public void SetPassword(string password) => 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password) => 
            BCrypt.Net.BCrypt.Verify(password, PasswordHash);

        public bool EsPro() => 
            PlanActual == PlanSuscripcion.ProMensual && FechaFinSuscripcion > DateTime.Now;

        public bool IsPro => EsPro(); // Alias for compatibility with services
    }
}
