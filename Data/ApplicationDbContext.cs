using Microsoft.EntityFrameworkCore;
using REPS_backend.Models;

namespace REPS_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Amistad> Amistades { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<DetalleMuscular> DetallesMusculares { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }
        public DbSet<Sesion> Sesiones { get; set; }
        public DbSet<RecordPersonal> RecordsPersonales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. CONFIGURACIÓN DE AMISTAD
            // Definimos que la ID es la pareja de (Solicitante + Receptor)
            modelBuilder.Entity<Amistad>()
                .HasKey(a => new { a.SolicitanteId, a.ReceptorId });

            // Relación con el Solicitante (El que pide amistad)
            modelBuilder.Entity<Amistad>()
                .HasOne(a => a.Solicitante)
                .WithMany()
                .HasForeignKey(a => a.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Relación con el Receptor (El que acepta)
            modelBuilder.Entity<Amistad>()
                .HasOne(a => a.Receptor)
                .WithMany()
                .HasForeignKey(a => a.ReceptorId)
                .OnDelete(DeleteBehavior.Restrict);


            // 2. CONFIGURACIÓN DE RUTINA-EJERCICIO (Tabla intermedia)
            // Es muy probable que necesites esto para evitar el error "Entity has no key defined"
            modelBuilder.Entity<RutinaEjercicio>()
                .HasKey(re => new { re.RutinaId, re.EjercicioId });
        }
    }
}