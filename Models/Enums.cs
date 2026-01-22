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
        Calentamiento = 1, 
        Aproximacion = 2,
        DropSet = 3,       
        AlFallo = 4
    }

    public enum EstadoRutina
    {
        Privada = 0,      
        EnRevision = 1,   
        Publicada = 2,    
        Rechazada = 3     
    }

    public enum NivelDificultad
    {
        Principiante = 0, 
        Intermedio = 1,
        Avanzado = 2      
    }
}
