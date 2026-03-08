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

    public enum GrupoMuscular
    {
        Pecho = 0,
        Espalda = 1,
        Pierna = 2,
        Hombro = 3,
        Biceps = 4,
        Triceps = 5,
        Abdomen = 6,
        Cardio = 7,
        FullBody = 8,
        Otro = 9
    }

    public enum Rango
    {
        Bronce = 0,
        Plata = 1,
        Oro = 2,
        Platino = 3,
        Diamante = 4,
        Leyenda = 5,
        Maestro = 6,
        GranMaestro = 7,
        Challenger = 8,
        Mitico = 9,
        Legendario = 10
    }
}
