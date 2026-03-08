using REPS_backend.Models;

namespace REPS_backend.DTOs.Ranking
{
    public class LeaderboardItemDto
    {
        public int Posicion { get; set; }
        public int CambioPosicion { get; set; } // +3, -1, 0
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string AvatarId { get; set; } = string.Empty; // URL or ID
        public int PuntosTotales { get; set; }
        public int PuntosRangoGeneral { get; set; }
        public int EntrenamientosTotal { get; set; } // Opcional, visto en Figma
        public string RangoGeneral { get; set; } = string.Empty;
    }

    public class MuscleProgressDto
    {
        public string GrupoMuscular { get; set; } = string.Empty;
        public string RangoActual { get; set; } = string.Empty; // Oro, Plata...
        public double CurrentPoints { get; set; } // Equivalente a kgs o puntos calculados
        public double NextRankThreshold { get; set; }
        public double ProgressPercentage { get; set; }
        public string NextRank { get; set; } = string.Empty;
    }

    public class UserProgressResponseDto
    {
        public int PuntosTotales { get; set; }
        public int PuntosRangoGeneral { get; set; }
        public string RangoGeneral { get; set; } = string.Empty;
        public int RachaDias { get; set; }
        public List<MuscleProgressDto> RangosMusculares { get; set; } = new();
    }
}
