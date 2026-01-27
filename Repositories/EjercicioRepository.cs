using Microsoft.EntityFrameworkCore;
using REPS_backend.Data;
using REPS_backend.Models;
namespace REPS_backend.Repositories
{
    public class EjercicioRepository : IEjercicioRepository
    {
        private readonly ApplicationDbContext _context;
        public EjercicioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // AQUÍ ESTÁ LA MAGIA DEL FILTRO
        public async Task<List<Ejercicio>> GetAllPersonalizadosAsync(int userId)
        {
            return await _context.Ejercicios
                .Where(e => e.UsuarioCreadorId == null || e.UsuarioCreadorId == userId)
                .ToListAsync();
        }

        public async Task<Ejercicio?> GetByIdAsync(int id)
        {
            return await _context.Ejercicios.FindAsync(id);
        }
        public async Task AddAsync(Ejercicio ejercicio)
        {
            await _context.Ejercicios.AddAsync(ejercicio);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var ejercicio = await _context.Ejercicios.FindAsync(id);
            if (ejercicio != null)
            {
                _context.Ejercicios.Remove(ejercicio);
                await _context.SaveChangesAsync();
            }
        }
    }
}
