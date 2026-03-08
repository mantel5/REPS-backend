using REPS_backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPS_backend.Repositories
{
    public interface IRecordPersonalRepository
    {
        Task<IEnumerable<RecordPersonal>> GetByUserIdAsync(int userId);
        Task<RecordPersonal?> GetBestByExerciseAsync(int userId, int ejercicioId);
        Task AddAsync(RecordPersonal record);
        Task UpdateAsync(RecordPersonal record);
    }
}
