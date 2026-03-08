using System;

namespace REPS_backend.DTOs.Logros
{
    public class LogroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Puntos { get; set; }
        public string IconoUrl { get; set; }
        public double Progreso { get; set; }
        public bool Desbloqueado { get; set; }
        public DateTime? FechaObtencion { get; set; }
    }
}
