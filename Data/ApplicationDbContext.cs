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
        public DbSet<SerieLog> SerieLogs { get; set; }
        public DbSet<RecordPersonal> RecordsPersonales { get; set; }

        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Amistad>()
                .HasKey(a => new { a.SolicitanteId, a.ReceptorId });

            modelBuilder.Entity<Amistad>()
                .HasOne(a => a.Solicitante)
                .WithMany()
                .HasForeignKey(a => a.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Amistad>()
                .HasOne(a => a.Receptor)
                .WithMany()
                .HasForeignKey(a => a.ReceptorId)
                .OnDelete(DeleteBehavior.Restrict);

            // CONFIGURACIÓN DE RUTINA-EJERCICIO
            modelBuilder.Entity<RutinaEjercicio>()
                .HasKey(re => new { re.RutinaId, re.EjercicioId });

            //CONFIGURACIÓN DE LIKES
            // Esto impide duplicados en la base de datos.
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UsuarioId, l.RutinaId })
                .IsUnique();

            // CONFIGURACIÓN DE LOGROS
            modelBuilder.Entity<UsuarioLogro>()
                .HasIndex(ul => new { ul.UsuarioId, ul.LogroId })
                .IsUnique();
        }

        public DbSet<Logro> Logros { get; set; }
        public DbSet<UsuarioLogro> UsuarioLogros { get; set; }
        public DbSet<Entrenamiento> Entrenamientos { get; set; }
    }
}