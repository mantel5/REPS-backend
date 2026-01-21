namespace RepsAPI.Models
{
    public enum GrupoMuscular
    {
        Pecho,
        Espalda,
        Hombro,
        Pierna,
        Brazo,
        Abdomen,
        Gluteos,
        Otros
    }

    public class Ejercicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public GrupoMuscular GrupoMuscular { get; set; }
        public NivelRutina Nivel { get; set; } // Reutilizamos el Enum de Rutina
        public string Imagen { get; set; } = string.Empty;

        public List<RutinaEjercicio> RutinaEjercicios { get; set; } = new List<RutinaEjercicio>();
    }
}