using REPS_backend.Models;

namespace REPS_backend.DTOs.Progreso
{
    public class ProgresoMuscularDto
    {
        public string GrupoMuscular { get; set; } = string.Empty;
        public string Rango { get; set; } = "SinRango";
        public int PuntosActuales { get; set; }
        public int PuntosParaSiguienteNivel { get; set; } // Cuántos faltan, o el target absoluto? Mejor target absoluto para la barra de progreso.
        public int SiguienteNivelPuntos { get; set; }
        public double Porcentaje { get; set; } // 0.0 a 1.0
        public int EntrenamientosRealizados { get; set; }
    }

    public class ProgresoGeneralDto
    {
        public string RangoGeneral { get; set; } = "SinRango";
        public int PuntosTotales { get; set; }
    }
}
