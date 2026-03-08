using Microsoft.EntityFrameworkCore;
using REPS_backend.Data;
using REPS_backend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REPS_backend.Repositories
{
    public class RecordPersonalRepository : IRecordPersonalRepository
    {
        private readonly ApplicationDbContext _context;

        public RecordPersonalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecordPersonal>> GetByUserIdAsync(int userId)
        {
            try 
            {
                return await _context.RecordsPersonales
                    .Include(r => r.Ejercicio)
                    .Where(r => r.UsuarioId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Loguear el error real
                Console.WriteLine($"ERROR en RecordPersonalRepository.GetByUserIdAsync: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
                throw;
            }
        }

        public async Task<RecordPersonal?> GetBestByExerciseAsync(int userId, int ejercicioId)
        {
            return await _context.RecordsPersonales
                .Where(r => r.UsuarioId == userId && r.EjercicioId == ejercicioId)
                .OrderByDescending(r => r.PesoMaximo) // Asumimos que el record es por peso por ahora
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(RecordPersonal record)
        {
            _context.RecordsPersonales.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RecordPersonal record)
        {
            _context.RecordsPersonales.Update(record);
            await _context.SaveChangesAsync();
        }
    }
}
