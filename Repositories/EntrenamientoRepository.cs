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

        public async Task<List<Entrenamiento>> GetByUsuarioIdWithSeriesAsync(int usuarioId)
        {
            return await _context.Entrenamientos
                .Include(e => e.SeriesRealizadas)
                    .ThenInclude(s => s.Ejercicio)
                .Where(e => e.UsuarioId == usuarioId)
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }

        public async Task<List<Entrenamiento>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Entrenamientos
                .Where(e => e.UsuarioId == usuarioId)
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalVolumeByUsuarioIdAsync(int usuarioId)
        {
            var totalVolume = await _context.Entrenamientos
                .Where(e => e.UsuarioId == usuarioId)
                .SelectMany(e => e.SeriesRealizadas)
                .Where(s => s.Completada)
                .SumAsync(s => (decimal?)s.PesoUsado * s.RepsRealizadas) ?? 0;
            return totalVolume;
        }

        public async Task<Dictionary<int, DateTime>> ObtenerUltimasFechasRutinasAsync(int usuarioId, List<int> rutinaIds)
        {
            var fechas = await _context.Entrenamientos
                .Where(e => e.UsuarioId == usuarioId && e.RutinaId.HasValue && rutinaIds.Contains(e.RutinaId.Value))
                .GroupBy(e => e.RutinaId!.Value)
                .Select(g => new { RutinaId = g.Key, UltimaFecha = g.Max(e => e.Fecha) })
                .ToListAsync();

            return fechas.ToDictionary(x => x.RutinaId, x => x.UltimaFecha);
        }
    }
}
