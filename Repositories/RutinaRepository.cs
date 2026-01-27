using Microsoft.EntityFrameworkCore;
using REPS_backend.Data;
using REPS_backend.Models;
namespace REPS_backend.Repositories
{
    public class RutinaRepository : IRutinaRepository
    {
        private readonly ApplicationDbContext _context;
        public RutinaRepository(ApplicationDbContext context) { _context = context; }
        
        public async Task AddAsync(Rutina rutina)
        {
            await _context.Rutinas.AddAsync(rutina);
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<Rutina>> GetAllPublicasAsync()
        {
            return await _context.Rutinas
                .Include(r => r.Usuario)
                .Include(r => r.Ejercicios)
                .Where(r => r.EsPublica)
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
        
        public async Task DeleteAsync(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina != null) { _context.Rutinas.Remove(rutina); await _context.SaveChangesAsync(); }
        }
    }
}
