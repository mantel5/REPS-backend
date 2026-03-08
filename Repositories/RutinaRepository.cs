using Microsoft.EntityFrameworkCore;
using REPS_backend.Data;
using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public class RutinaRepository : IRutinaRepository
    {
        private readonly ApplicationDbContext _context;

        public RutinaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Rutina rutina)
        {
            await _context.Rutinas.AddAsync(rutina);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Rutina>> GetAllPublicasAsync()
        {
            // Traemos solo las publicadas e incluimos los ejercicios con su detalle
            return await _context.Rutinas
                .Where(r => r.Estado == EstadoRutina.Publicada)
                .Include(r => r.Ejercicios)
                    .ThenInclude(re => re.Ejercicio)
                .Include(r => r.Usuario)
                .ToListAsync();
        }

        public async Task<List<Rutina>> GetAllEnRevisionAsync()
        {
            return await _context.Rutinas
                .Where(r => r.Estado == EstadoRutina.EnRevision)
                .Include(r => r.Ejercicios)
                    .ThenInclude(re => re.Ejercicio)
                .Include(r => r.Usuario)
                .ToListAsync();
        }

        public async Task<List<Rutina>> GetAllAdminAsync()
        {
            return await _context.Rutinas
                .Include(r => r.Ejercicios)
                    .ThenInclude(re => re.Ejercicio)
                .Include(r => r.Usuario)
                .ToListAsync();
        }

        public async Task<Rutina?> GetByIdAsync(int id)
        {
            // Buscamos por ID e incluimos los ejercicios
            return await _context.Rutinas
                .Include(r => r.Ejercicios)
                    .ThenInclude(re => re.Ejercicio)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rutina?> GetByIdWithEjerciciosAsync(int id)
        {
            return await _context.Rutinas
                .Include(r => r.Ejercicios)
                    .ThenInclude(re => re.Ejercicio)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Rutina>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Rutinas
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Ejercicios)
                    .ThenInclude(re => re.Ejercicio)
                .ToListAsync();
        }
        public async Task UpdateAsync(Rutina rutina)
        {
            _context.Rutinas.Update(rutina);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rutina = await _context.Rutinas
                .Include(r => r.Ejercicios)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (rutina != null)
            {
                var entrenamientos = _context.Entrenamientos.Where(e => e.RutinaId == id);
                foreach (var e in entrenamientos)
                {
                    e.RutinaId = null;
                }
                
                _context.Rutinas.Remove(rutina);
                await _context.SaveChangesAsync();
            }
        }
    }
}