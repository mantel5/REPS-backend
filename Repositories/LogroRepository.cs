using Microsoft.EntityFrameworkCore;
using REPS_backend.Data;
using REPS_backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace REPS_backend.Repositories
{
    public class LogroRepository : ILogroRepository
    {
        private readonly ApplicationDbContext _context;

        public LogroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Logro>> GetAllAsync()
        {
            return await _context.Logros.ToListAsync();
        }

        public async Task<Logro?> GetByIdAsync(int id)
        {
            return await _context.Logros.FindAsync(id);
        }

        public async Task<Logro> AddAsync(Logro logro)
        {
            _context.Logros.Add(logro);
            await _context.SaveChangesAsync();
            return logro;
        }

        public async Task<IEnumerable<UsuarioLogro>> GetUserLogrosAsync(int userId)
        {
            return await _context.UsuarioLogros
                .Include(ul => ul.Logro)
                .Where(ul => ul.UsuarioId == userId)
                .ToListAsync();
        }

        public async Task AddUsuarioLogroAsync(UsuarioLogro usuarioLogro)
        {
            await _context.UsuarioLogros.AddAsync(usuarioLogro);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUsuarioLogroAsync(UsuarioLogro usuarioLogro)
        {
            _context.UsuarioLogros.Update(usuarioLogro);
            await _context.SaveChangesAsync();
        }
    }
}
