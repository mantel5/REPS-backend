using Microsoft.EntityFrameworkCore;
using REPS_backend.Models;

namespace REPS_backend.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // 1. Crea la base de datos si no existe
            context.Database.EnsureCreated();

            // Seed Usuario Admin
            var admin = context.Usuarios.FirstOrDefault(u => u.Email == "admin@reps.com");
            if (admin == null)
            {
                admin = new Usuario
                {
                    Nombre = "Admin",
                    Email = "admin@reps.com",
                    Rol = Rol.Admin,
                    PlanActual = PlanSuscripcion.Gratuito,
                    FechaRegistro = DateTime.UtcNow,
                    PuntosTotales = 0,
                    RachaDias = 0,
                    CodigoAmigo = "ADMIN01"
                };
                admin.SetPassword("Admin123!");
                context.Usuarios.Add(admin);
                context.SaveChanges();
            }

            var dani = context.Usuarios.FirstOrDefault(u => u.Email == "Dani@gmail.com");
            if (dani != null) {
                dani.SetPassword("Daniel123");
                context.SaveChanges();
            }

            var hola = context.Usuarios.FirstOrDefault(u => u.Email == "hola@gmail.com");
            if (hola == null) {
                hola = new Usuario {
                    Nombre = "Hola",
                    Email = "hola@gmail.com"
                };
                hola.SetPassword("Daniel123");
                context.Usuarios.Add(hola);
                context.SaveChanges();
            } else {
                hola.SetPassword("Daniel123");
                context.SaveChanges();
            }

            // Set points for hola user
            if (hola != null) {
                hola.PuntosTotales = 3500;
                hola.PuntosLogros = 0;
                hola.RangoGeneral = Rango.Oro;
                context.SaveChanges();
            }

            var ejercicios = new List<Ejercicio>
            {
               new Ejercicio { Nombre = "Press de Banca", GrupoMuscular = GrupoMuscular.Pecho, DescripcionTecnica = "Barra al pecho.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Sentadilla", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Rompe el paralelo.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Peso Muerto", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Espalda neutra.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Dominadas", GrupoMuscular = GrupoMuscular.Espalda, DescripcionTecnica = "Barbilla arriba.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Remo Barra", GrupoMuscular = GrupoMuscular.Espalda, DescripcionTecnica = "Tirón a cadera.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Press Militar", GrupoMuscular = GrupoMuscular.Hombro, DescripcionTecnica = "Barra sobre cabeza.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Curl Bíceps", GrupoMuscular = GrupoMuscular.Biceps, DescripcionTecnica = "Codos pegados.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Fondos Tríceps", GrupoMuscular = GrupoMuscular.Triceps, DescripcionTecnica = "Baja controlado.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Zancadas", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Rodilla al suelo.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Plancha", GrupoMuscular = GrupoMuscular.Abdomen, DescripcionTecnica = "Isometría.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               // 20 nuevos ejercicios añadidos
               new Ejercicio { Nombre = "Press Inclinado", GrupoMuscular = GrupoMuscular.Pecho, DescripcionTecnica = "Enfoque en pecho superior.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Aperturas con Mancuernas", GrupoMuscular = GrupoMuscular.Pecho, DescripcionTecnica = "Estira el pecho.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Cruce de Poleas", GrupoMuscular = GrupoMuscular.Pecho, DescripcionTecnica = "Tensión constante.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Prensa de Piernas", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Empuja con las rodillas alineadas.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Curl Femoral", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Flexión de rodilla.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Extensiones de Cuádriceps", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Extensión completa de rodilla.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Elevaciones de Gemelos", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Pausa arriba.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Jalón al Pecho", GrupoMuscular = GrupoMuscular.Espalda, DescripcionTecnica = "Tira con los dorsales.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Remo Gironda", GrupoMuscular = GrupoMuscular.Espalda, DescripcionTecnica = "Mantén la espalda recta.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Pull-over", GrupoMuscular = GrupoMuscular.Espalda, DescripcionTecnica = "Estiramiento dorsal.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Elevaciones Laterales", GrupoMuscular = GrupoMuscular.Hombro, DescripcionTecnica = "Levanta los codos.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Pájaros (Deltoides Posterior)", GrupoMuscular = GrupoMuscular.Hombro, DescripcionTecnica = "Enfoque en la parte posterior.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Encogimientos de Hombros", GrupoMuscular = GrupoMuscular.Hombro, DescripcionTecnica = "Sube los trapecios.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Curl Martillo", GrupoMuscular = GrupoMuscular.Biceps, DescripcionTecnica = "Agarre neutro.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Curl Predicador", GrupoMuscular = GrupoMuscular.Biceps, DescripcionTecnica = "Aisla el bíceps.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Extensión de Tríceps Polea", GrupoMuscular = GrupoMuscular.Triceps, DescripcionTecnica = "Tira hacia abajo.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Press Francés", GrupoMuscular = GrupoMuscular.Triceps, DescripcionTecnica = "Codos fijos hacia el techo.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Crunch Abdominal", GrupoMuscular = GrupoMuscular.Abdomen, DescripcionTecnica = "No tires del cuello.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Elevación de Piernas Colgado", GrupoMuscular = GrupoMuscular.Abdomen, DescripcionTecnica = "Controla el balanceo.", ImagenMusculosUrl = "url", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Russian Twists", GrupoMuscular = GrupoMuscular.Abdomen, DescripcionTecnica = "Gira el torso.", ImagenMusculosUrl = "url", UsuarioCreadorId = null }
            };

            foreach (var ejercicio in ejercicios)
            {
                if (!context.Ejercicios.Any(e => e.Nombre == ejercicio.Nombre))
                {
                    context.Ejercicios.Add(ejercicio);
                }
            }
            context.SaveChanges();



            // 4. Seeding Logros (Verificamos uno por uno para no duplicar ni saltar si ya hay otros)
            var logrosDefault = new Logro[]
            {
                new Logro { Titulo = "Primeros Pasos", Descripcion = "Completa tu primer entrenamiento.", Puntos = 50, IconoUrl = "flame" },
                new Logro { Titulo = "Constancia", Descripcion = "Completa 3 sesiones en una semana.", Puntos = 100, IconoUrl = "crown" },
                new Logro { Titulo = "Racha de Fuego", Descripcion = "28 días consecutivos", Puntos = 300, IconoUrl = "flame" },
                new Logro { Titulo = "Centurión", Descripcion = "100 entrenamientos totales", Puntos = 150, IconoUrl = "crown" },
                new Logro { Titulo = "Estrella Ascendente", Descripcion = "50 entrenamientos / 30 días", Puntos = 100, IconoUrl = "star" },
                new Logro { Titulo = "Velocista", Descripcion = "10 entrenamientos / semana", Puntos = 80, IconoUrl = "bolt" },
                new Logro { Titulo = "Inquebrantable", Descripcion = "60 días de actividad", Puntos = 300, IconoUrl = "target" },
                new Logro { Titulo = "Guerrero del Hierro", Descripcion = "Completa 100 entrenamientos", Puntos = 200, IconoUrl = "muscle" },
                new Logro { Titulo = "Maestro del Volumen", Descripcion = "Levanta un total de 100,000 kg", Puntos = 300, IconoUrl = "target" },
                new Logro { Titulo = "Coleccionista de Records", Descripcion = "Establece 5 récords personales", Puntos = 180, IconoUrl = "bolt" },
                new Logro { Titulo = "Perfeccionista", Descripcion = "Completa 50 entrenamientos sin fallar", Puntos = 150, IconoUrl = "star" }
            };

            foreach (var logro in logrosDefault)
            {
                if (!context.Logros.Any(l => l.Titulo == logro.Titulo))
                {
                    context.Logros.Add(logro);
                }
            }

            context.SaveChanges();
        }
    }
}