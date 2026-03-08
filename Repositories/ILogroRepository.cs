using REPS_backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPS_backend.Repositories
{
    public interface ILogroRepository
    {
        Task<IEnumerable<Logro>> GetAllAsync();
        Task<Logro?> GetByIdAsync(int id);
        Task<Logro> AddAsync(Logro logro);
        Task<IEnumerable<UsuarioLogro>> GetUserLogrosAsync(int userId);
        Task AddUsuarioLogroAsync(UsuarioLogro usuarioLogro);
        Task UpdateUsuarioLogroAsync(UsuarioLogro usuarioLogro);
    }
}
