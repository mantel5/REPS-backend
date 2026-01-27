namespace REPS_backend.DTOs.Rutinas
{
    public class RutinaUpdateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool EsPublica { get; set; }
    }
}
