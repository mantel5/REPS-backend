using Microsoft.EntityFrameworkCore;
using REPS_backend.Data;
using REPS_backend.Models;
namespace REPS_backend.Repositories
{
    public class RutinaEjercicioRepository : IRutinaEjercicioRepository
    {
        private readonly ApplicationDbContext _context;
        public RutinaEjercicioRepository(ApplicationDbContext context) { _context = context; }

        public async Task<RutinaEjercicio?> GetByIdAsync(int id)
        {
            return await _context.RutinaEjercicios.Include(re => re.Ejercicio).FirstOrDefaultAsync(re => re.Id == id);
        }
        public async Task AddAsync(RutinaEjercicio rutinaEjercicio)
        {
            await _context.RutinaEjercicios.AddAsync(rutinaEjercicio);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(RutinaEjercicio rutinaEjercicio)
        {
            _context.RutinaEjercicios.Update(rutinaEjercicio);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.RutinaEjercicios.FindAsync(id);
            if (entity != null)
            {
                _context.RutinaEjercicios.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
