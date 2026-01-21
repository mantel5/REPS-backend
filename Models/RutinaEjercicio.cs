namespace RepsAPI.Models
{
    public class RutinaEjercicio
    {
        public int RutinaId { get; set; }
        public Rutina Rutina { get; set; }

        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }

        public int Series { get; set; }
        public string Repeticiones { get; set; } = string.Empty;
        public decimal Peso { get; set; }
        public int DescansoSegundos { get; set; }
    }
}