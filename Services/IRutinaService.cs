using REPS_backend.DTOs.Rutinas;
namespace REPS_backend.Services
{
    public interface IRutinaService
    {
        Task<RutinaDetalleDto> CrearRutinaAsync(RutinaCreateDto dto, int usuarioId);
        Task<List<RutinaItemDto>> ObtenerRutinasPublicasAsync();
        Task<RutinaDetalleDto?> ObtenerDetalleRutinaAsync(int id);
        Task<bool> BorrarRutinaAsync(int id, int usuarioId);
    }
}
