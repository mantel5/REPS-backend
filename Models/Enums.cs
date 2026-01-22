namespace REPS_backend.Models
{
    public enum PlanSuscripcion
    {
        Gratuito = 0,
        ProMensual = 1 
    }

    public enum TipoSerie
    {
        Normal = 0,
        Calentamiento = 1, // Usa porcentaje
        Aproximacion = 2,
        DropSet = 3,       // Usa porcentaje
        AlFallo = 4
    }

    public enum EstadoRutina
    {
        Privada = 0,      // Solo la ve el dueño
        EnRevision = 1,   // Esperando aprobación
        Publicada = 2,    // Visible para todos
        Rechazada = 3     // Denegada
    }
}