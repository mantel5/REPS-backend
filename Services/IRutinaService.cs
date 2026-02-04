using REPS_backend.DTOs.Rutinas;
using REPS_backend.DTOs.Ejercicios;
using REPS_backend.Models;
using REPS_backend.Repositories;

namespace REPS_backend.Services
{
    public interface IRutinaService
    {
        Task<RutinaDetalleDto> CrearRutinaAsync(RutinaCreateDto dto, int usuarioId);
        Task<bool> ActualizarRutinaAsync(int id, RutinaUpdateDto dto, int usuarioId); 
        Task<List<RutinaItemDto>> ObtenerRutinasPublicasAsync();
        Task<RutinaDetalleDto?> ObtenerDetalleRutinaAsync(int id);
        Task<bool> BorrarRutinaAsync(int id, int usuarioId);

        Task<bool> EnviarARevisionAsync(int rutinaId, int usuarioId);
        Task<IEnumerable<Rutina>> ObtenerRutinasPendientesAsync();
        Task<bool> ValidarRutinaAsync(int rutinaId, bool aprobar);
        Task<bool> BanearRutinaAsync(int rutinaId);

        Task<List<RutinaDetalleDto>> ObtenerRutinasDeUsuarioAsync(int usuarioId);
        Task<bool> ToggleLikeAsync(int rutinaId, int usuarioId);
        
    }
}