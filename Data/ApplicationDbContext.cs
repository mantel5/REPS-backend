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
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<DetalleMuscular> DetallesMusculares { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }
        public DbSet<Sesion> Sesiones { get; set; }
        public DbSet<RecordPersonal> RecordsPersonales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

// Explicación:
// ApplicationDbContext es la clase que representa la base de datos en EF Core.
// Cada DbSet<> es una tabla en la base de datos.