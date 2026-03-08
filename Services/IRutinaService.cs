using REPS_backend.DTOs.Rutinas;

namespace REPS_backend.Services
{
    public interface IRutinaService
    {
        Task<RutinaDetalleDto> CrearRutinaAsync(RutinaCreateDto dto, int usuarioId);
        Task<List<RutinaItemDto>> ObtenerRutinasPublicasAsync();
        Task<List<RutinaItemDto>> ObtenerRutinasUsuarioAsync(int usuarioId);
        Task<RutinaDetalleDto> ObtenerDetalleRutinaAsync(int rutinaId, int usuarioId = 0);
        Task<RutinaDetalleDto> GenerarRutinaIAAsync(RutinaIARequestDto dto, int usuarioId);
        Task<int> LikeRutinaAsync(int rutinaId);
        Task<RutinaDetalleDto> CopiarRutinaAsync(int rutinaId, int usuarioId);
        Task<bool> EliminarRutinaAsync(int id, int usuarioId);
        Task<bool> EliminarRutinaAdminAsync(int id);
        Task<bool> PublicarRutinaAsync(int id, int usuarioId);
        Task<bool> ValidarRutinaAsync(int id);
        Task<bool> RechazarRutinaAsync(int id);
        Task<List<RutinaItemDto>> ObtenerRutinasEnRevisionAsync();
        Task<List<RutinaItemDto>> ObtenerTodasRutinasAdminAsync();
        Task<RutinaDetalleDto> ActualizarRutinaAsync(int id, RutinaCreateDto dto, int usuarioId);
    }
}