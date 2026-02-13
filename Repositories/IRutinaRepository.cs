using REPS_backend.Models;

namespace REPS_backend.Repositories
{
    public interface IRutinaRepository
    {
        Task<List<Rutina>> GetAllAsync();
        Task<List<Rutina>> GetAllPublicasAsync();
        Task<List<Rutina>> GetByUsuarioIdAsync(int usuarioId, NivelDificultad? nivel = null, GrupoMuscular? musculo = null);
        Task<Rutina?> GetByIdWithEjerciciosAsync(int id);
        Task<Rutina?> GetByIdSimpleAsync(int id);
        Task<Rutina?> GetByIdAsync(int id);
        Task AddAsync(Rutina rutina);
        Task UpdateAsync(Rutina rutina);
        Task DeleteAsync(int id);
        Task<Like?> ObtenerLikeAsync(int rutinaId, int usuarioId);
        Task AddLikeAsync(Like like);
        Task RemoveLikeAsync(Like like);
        Task<List<Rutina>> GetLikedByUserIdAsync(int usuarioId);
    }
}