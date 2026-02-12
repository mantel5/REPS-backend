using REPS_backend.Models;

namespace REPS_backend.DTOs.Sesiones
{
    public class SesionCreateDto
    {
        public int RutinaId { get; set; }
        public int DuracionRealMinutos { get; set; }
        public List<SerieLogDto> SeriesRealizadas { get; set; } = new List<SerieLogDto>();
    }

    public class SerieLogDto
    {
        public int EjercicioId { get; set; }
        public int NumeroSerie { get; set; }
        public int Reps { get; set; }
        public decimal Peso { get; set; }
        public bool Completada { get; set; }
    }
}
