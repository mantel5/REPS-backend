namespace REPS_backend.Models
{
    public enum PlanSuscripcion { Gratuito, ProMensual }

    public enum TipoSerie { Normal, Calentamiento, Aproximacion, DropSet, AlFallo }

    public enum EstadoRutina { Privada, EnRevision, Publicada, Rechazada }

    public enum NivelDificultad { Principiante, Intermedio, Avanzado }

    public enum GrupoMuscular 
    {
        Pecho, Espalda, Pierna, Hombro, Biceps, Triceps, Abdomen, Cardio, FullBody, Otro
    }
}