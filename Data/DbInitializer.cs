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
            if (context.Ejercicios.Any())
            {
                return; 
            }

            // 3. Si está vacía, metemos los ejercicios básicos
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
    }
}