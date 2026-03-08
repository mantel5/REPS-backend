namespace REPS_backend.Models
{
    public class RecordPersonal
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EjercicioId { get; set; } 
        public virtual Ejercicio Ejercicio { get; set; } = null!;

        public decimal PesoMaximo { get; set; } 
        public DateTime FechaRecord { get; set; } 
        public decimal PesoAnterior { get; set; } 
    }
}
