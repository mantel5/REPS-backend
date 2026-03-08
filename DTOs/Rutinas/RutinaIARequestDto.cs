using System.Collections.Generic;

namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaIARequestDto
    {
        public string Goal { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int Days { get; set; }
        public string Duration { get; set; } = string.Empty;
        public List<string> Muscles { get; set; } = new();
        public List<string> Equipment { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
    }
}
