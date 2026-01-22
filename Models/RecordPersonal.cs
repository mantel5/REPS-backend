namespace REPS_backend.Models
{
    public class RecordPersonal
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EjercicioId { get; set; } 

        // Récord actual (Ej: 140 kg)
        public decimal PesoMaximo { get; set; } 
        
        // Fecha del récord (Para "Hace 3 días")
        public DateTime FechaRecord { get; set; } 

        // Récord anterior (Para calcular la mejora: +10kg en verde)
        public decimal PesoAnterior { get; set; } 
    }
}