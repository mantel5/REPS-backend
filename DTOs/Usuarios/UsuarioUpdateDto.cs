namespace REPS_backend.DTOs.Usuarios
{
    public class UsuarioUpdateDto
    {
        public string? Nombre { get; set; }
        public string? AvatarId { get; set; }
        public string? Biografia { get; set; }
        public bool? EsPerfilPublico { get; set; }
        public bool? MostrarEstadisticas { get; set; }
        public bool? RankingVisible { get; set; }
    }
}