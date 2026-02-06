using REPS_backend.DTOs; // Suggest creating this if not exists or use simple DTO
using System.Threading.Tasks;

namespace REPS_backend.Services
{
    public interface IRecordPersonalService
    {
        Task<bool> RegistrarNuevoLevantamientoAsync(int userId, int ejercicioId, double peso);
    }
}
