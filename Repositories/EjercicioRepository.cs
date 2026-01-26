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

        public async Task<Ejercicio> AddAsync(Ejercicio ejercicio)
        {
            await _context.Ejercicios.AddAsync(ejercicio);
            await _context.SaveChangesAsync();
            return ejercicio;
        }

        public async Task<List<Ejercicio>> GetAllAsync()
        {
            return await _context.Ejercicios.ToListAsync();
        }

        public async Task<Ejercicio?> GetByIdAsync(int id)
        {
            return await _context.Ejercicios.FindAsync(id);
        }
    }
}