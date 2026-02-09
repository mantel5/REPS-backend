using REPS_backend.DTOs.Records;
using System.Threading.Tasks;

namespace REPS_backend.Services
{
    public interface IRecordPersonalService
    {
        Task<bool> RegistrarNuevoLevantamientoAsync(int userId, int ejercicioId, decimal peso);
        Task<List<RecordPersonalDto>> ObtenerRecordsUsuarioAsync(int userId);
    }
}
