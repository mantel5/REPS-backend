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


            // 2. Definimos los ejercicios básicos
            var ejerciciosDefault = new Ejercicio[]
            {
               new Ejercicio { Nombre = "Press de Banca", GrupoMuscular = GrupoMuscular.Pecho, DescripcionTecnica = "Barra al pecho.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/video/upload/v1772039401/Video_Generado_Sin_Fuego_l8s9iz.mp4", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Sentadilla", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Rompe el paralelo.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Peso Muerto", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Espalda neutra.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Dominadas", GrupoMuscular = GrupoMuscular.Espalda, DescripcionTecnica = "Barbilla arriba.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Remo Barra", GrupoMuscular = GrupoMuscular.Espalda, DescripcionTecnica = "Tirón a cadera.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Press Militar", GrupoMuscular = GrupoMuscular.Hombro, DescripcionTecnica = "Barra sobre cabeza.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Curl Bíceps", GrupoMuscular = GrupoMuscular.Biceps, DescripcionTecnica = "Codos pegados.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Fondos Tríceps", GrupoMuscular = GrupoMuscular.Triceps, DescripcionTecnica = "Baja controlado.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Zancadas", GrupoMuscular = GrupoMuscular.Pierna, DescripcionTecnica = "Rodilla al suelo.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
               new Ejercicio { Nombre = "Plancha", GrupoMuscular = GrupoMuscular.Abdomen, DescripcionTecnica = "Isometría.", ImagenMusculosUrl = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772038108/descarga_w22ggj.jpg", UsuarioCreadorId = null },
            };

            // 3. Si no hay ejercicios, los metemos todos
            if (!context.Ejercicios.Any())
            {
                context.Ejercicios.AddRange(ejerciciosDefault);
                context.SaveChanges();
            }
            else
            {
                // Actualizar las URLs de los ejercicios por defecto existentes sincronizándolas con la lista actual
                var ejerciciosExistentes = context.Ejercicios.Where(e => e.UsuarioCreadorId == null).ToList();
                bool actualizados = false;

                foreach (var ejDef in ejerciciosDefault)
                {
                    var ejExistente = ejerciciosExistentes.FirstOrDefault(e => e.Nombre == ejDef.Nombre);
                    if (ejExistente != null && ejExistente.ImagenMusculosUrl != ejDef.ImagenMusculosUrl)
                    {
                        ejExistente.ImagenMusculosUrl = ejDef.ImagenMusculosUrl;
                        actualizados = true;
                    }
                }

                if (actualizados)
                {
                    context.SaveChanges();
                }
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