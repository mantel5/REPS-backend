using Microsoft.EntityFrameworkCore;
using REPS_backend.Data;
using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public class EntrenamientoRepository : IEntrenamientoRepository
    {
        private readonly ApplicationDbContext _context;

        public EntrenamientoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Entrenamiento entrenamiento)
        {
            await _context.Entrenamientos.AddAsync(entrenamiento);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Entrenamientos.CountAsync(e => e.UsuarioId == usuarioId);
        }
    }
}
