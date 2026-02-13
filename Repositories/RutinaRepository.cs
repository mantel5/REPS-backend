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

        public async Task UpdateAsync(Rutina rutina)
        {
            _context.Rutinas.Update(rutina);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina != null)
            {
                _context.Rutinas.Remove(rutina);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Rutina>> GetAllPublicasAsync()
        {
            return await _context.Rutinas
                .Include(r => r.Usuario)
                .Include(r => r.Ejercicios)
                .Where(r => r.EsPublica)
                .ToListAsync();
        }

        public async Task<List<Rutina>> GetAllAsync()
        {
            return await _context.Rutinas
                .Include(r => r.Usuario)
                .Include(r => r.Ejercicios)
                    .ThenInclude(re => re.Ejercicio)
                .ToListAsync();
        }

        public async Task<List<Rutina>> GetByUsuarioIdAsync(int usuarioId, NivelDificultad? nivel = null, GrupoMuscular? musculo = null)
        {
            var query = _context.Rutinas
                .Include(r => r.Ejercicios)
                    .ThenInclude(re => re.Ejercicio)
                .Where(r => r.UsuarioId == usuarioId);

            if (nivel.HasValue)
            {
                query = query.Where(r => r.Nivel == nivel.Value);
            }

            // Nota: Para filtrar por grupo muscular, buscamos si AL MENOS UN ejercicio de la rutina es de ese grupo.
            if (musculo.HasValue)
            {
                query = query.Where(r => r.Ejercicios.Any(re => re.Ejercicio!.GrupoMuscular == musculo.Value));
            }

            return await query
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        public async Task<Rutina?> GetByIdWithEjerciciosAsync(int id)
        {
            return await _context.Rutinas
                .Include(r => r.Usuario)
                .Include(r => r.Ejercicios)
                .ThenInclude(re => re.Ejercicio)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rutina?> GetByIdSimpleAsync(int id)
        {
            return await _context.Rutinas.FindAsync(id);
        }

        public async Task<Rutina?> GetByIdAsync(int id)
        {
            return await _context.Rutinas.FindAsync(id);
        }

        public async Task<Like?> ObtenerLikeAsync(int rutinaId, int usuarioId)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(l => l.RutinaId == rutinaId && l.UsuarioId == usuarioId);
        }

        public async Task AddLikeAsync(Like like)
        {
            await _context.Likes.AddAsync(like);

            var rutina = await _context.Rutinas.FindAsync(like.RutinaId);
            if (rutina != null)
            {
                rutina.Likes++;
                _context.Entry(rutina).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLikeAsync(Like like)
        {
            _context.Likes.Remove(like);

            var rutina = await _context.Rutinas.FindAsync(like.RutinaId);
            if (rutina != null)
            {
                rutina.Likes--;
                if (rutina.Likes < 0) rutina.Likes = 0;
                _context.Entry(rutina).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
        }

        public async Task<List<Rutina>> GetLikedByUserIdAsync(int usuarioId)
        {
            return await _context.Likes
                .Where(l => l.UsuarioId == usuarioId && l.Rutina != null)
                .Select(l => l.Rutina!)
                .Include(r => r.Usuario)
                .Include(r => r.Ejercicios!)
                    .ThenInclude(re => re.Ejercicio)
                .ToListAsync();
        }
    }
}