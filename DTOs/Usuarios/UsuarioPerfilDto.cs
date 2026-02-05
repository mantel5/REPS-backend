namespace REPS_backend.DTOs.Usuarios
{
    public class UsuarioPerfilDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; 
        public string AvatarId { get; set; } = string.Empty;
        public string CodigoAmigo { get; set; } = string.Empty; 
        public DateTime FechaRegistro { get; set; } 
        public string Rol { get; set; } = string.Empty;

    
        public int PuntosTotales { get; set; }
        public int RachaDias { get; set; }
        public string RangoGeneral { get; set; } = string.Empty;
        public bool EsPro { get; set; }
    }
}