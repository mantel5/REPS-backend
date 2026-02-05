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

            // 2. Si ya hay ejercicios, no hacemos nada


            // 2. Si hay ejercicios, metemos los básicos (pero solo si está vacía)
            if (!context.Ejercicios.Any())
            {
                var ejercicios = new Ejercicio[]
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
                };
                context.Ejercicios.AddRange(ejercicios);
                context.SaveChanges();
            }



            // 4. Seeding Logros
            // 4. Seeding Logros (Verificamos uno por uno para no duplicar ni saltar si ya hay otros)
            var logrosDefault = new Logro[]
            {
                new Logro { Titulo = "Primeros Pasos", Descripcion = "Completar la primera sesión.", Puntos = 10, IconoUrl = "trophy_start.png" },
                new Logro { Titulo = "Constancia", Descripcion = "Completar 3 sesiones en una semana.", Puntos = 20, IconoUrl = "trophy_calendar.png" },
                new Logro { Titulo = "Fuerza Bruta", Descripcion = "Registrar una sentadilla con peso corporal.", Puntos = 30, IconoUrl = "trophy_muscle.png" },
                new Logro { Titulo = "Social", Descripcion = "Añadir al primer amigo.", Puntos = 15, IconoUrl = "trophy_friends.png" },
                new Logro { Titulo = "Popular", Descripcion = "Recibir 10 likes en una rutina.", Puntos = 50, IconoUrl = "trophy_star.png" },
                new Logro { Titulo = "Creador", Descripcion = "Crear una rutina pública.", Puntos = 25, IconoUrl = "trophy_pencil.png" },
                new Logro { Titulo = "Maratoniano", Descripcion = "Entrenar más de 100 minutos en total.", Puntos = 40, IconoUrl = "trophy_runner.png" },
                new Logro { Titulo = "Imparable", Descripcion = "Racha de 7 días consecutivos.", Puntos = 100, IconoUrl = "trophy_fire.png" },
                new Logro { Titulo = "Veterano", Descripcion = "Alcanzar el nivel 10.", Puntos = 200, IconoUrl = "trophy_medal.png" },
                new Logro { Titulo = "Crítico", Descripcion = "Dar Like a 3 rutinas de la comunidad.", Puntos = 15, IconoUrl = "trophy_heart.png" },
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