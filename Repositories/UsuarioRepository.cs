using Microsoft.EntityFrameworkCore;
using REPS_backend.Data;
using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> GetByCodigoAmigoAsync(string codigo)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CodigoAmigo == codigo 
                                       && !u.EstaBorrado 
                                       && u.EstaActivo);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task CrearUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SonAmigosAsync(int usuarioId1, int usuarioId2)
        {
            return await _context.Amistades.AnyAsync(a =>
                (a.SolicitanteId == usuarioId1 && a.ReceptorId == usuarioId2) ||
                (a.SolicitanteId == usuarioId2 && a.ReceptorId == usuarioId1));
        }

        public async Task AgregarAmistadAsync(Amistad amistad)
        {
            await _context.Amistades.AddAsync(amistad);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuario>> GetAmigosDeUsuarioAsync(int usuarioId)
        {
            // 1. Busca filas donde seas el Solicitante O el Receptor.
            // 2. Si eres el Solicitante, te devuelve al Receptor. Si eres Receptor, te devuelve al Solicitante.
            // 3. Resultado: Una lista con tus amigos, sin importan quién agregó a quién.
            
            return await _context.Amistades
                .Where(a => a.SolicitanteId == usuarioId || a.ReceptorId == usuarioId)
                .Select(a => a.SolicitanteId == usuarioId ? a.Receptor : a.Solicitante)
                .ToListAsync();
        }
    }
}