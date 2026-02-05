using REPS_backend.DTOs.Logros;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPS_backend.Services
{
    public interface ILogroService
    {
        Task<List<LogroDTO>> GetLogrosForUserAsync(int userId);
        Task<LogroDTO> CreateLogroAsync(CreateLogroDTO dto);
        Task<List<LogroDTO>> GetAllAsync();
    }
}
