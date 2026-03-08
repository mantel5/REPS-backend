using REPS_backend.DTOs.Ejercicios;
using REPS_backend.DTOs.Rutinas;
using REPS_backend.DTOs.Logros;

namespace REPS_backend.DTOs.Dashboard
{
    public class DashboardResumenDto
    {
        public int RachaDias { get; set; }
        public string Nivel { get; set; } = "Novato";
        public int LogrosDesbloqueados { get; set; }
        public int TotalLogros { get; set; }

        public List<LogroDTO> UltimosLogros { get; set; } = new();
        public RutinaItemDto? RutinaSugerida { get; set; }
    }
}
