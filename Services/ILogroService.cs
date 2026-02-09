using REPS_backend.DTOs.Logros;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPS_backend.Services
{
    public interface ILogroService
    {
        Task<List<LogroDTO>> GetLogrosForUserAsync(int userId);
        Task<LogroDTO> CreateLogroAsync(CreateLogroDTO dto);
        Task<bool> UnlockLogroAsync(int userId, int logroId);
        Task<List<LogroDTO>> GetAllAsync();
        Task<List<LogroDTO>> GetUltimosLogrosDesbloqueadosAsync(int userId, int count);
    }
}
