using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;
using REPS_backend.Repositories;
namespace REPS_backend.Services
{
    public class RutinaEjercicioService : IRutinaEjercicioService
    {
        private readonly IRutinaEjercicioRepository _repo;
        private readonly IRutinaRepository _rutinaRepo; 

        public RutinaEjercicioService(IRutinaEjercicioRepository repo, IRutinaRepository rutinaRepo)
        {
            _repo = repo;
            _rutinaRepo = rutinaRepo;
        }

        public async Task<bool> AgregarEjercicioARutinaAsync(RutinaEjercicioAddDto dto, int usuarioId)
        {
            var rutina = await _rutinaRepo.GetByIdWithEjerciciosAsync(dto.RutinaId);
            if (rutina == null || rutina.UsuarioId != usuarioId) return false;

            var nuevo = new RutinaEjercicio
            {
                RutinaId = dto.RutinaId,
                EjercicioId = dto.EjercicioId,
                Series = dto.Series,
                Repeticiones = dto.Repeticiones,
                Orden = rutina.Ejercicios.Count + 1,
                DescansoSegundos = 60
            };

            await _repo.AddAsync(nuevo);
            return true;
        }

        public async Task<bool> ActualizarRutinaEjercicioAsync(int id, RutinaEjercicioUpdateDto dto, int usuarioId)
        {
            var rutinaEjercicio = await _repo.GetByIdAsync(id);
            if (rutinaEjercicio == null) return false;

            var rutina = await _rutinaRepo.GetByIdWithEjerciciosAsync(rutinaEjercicio.RutinaId);
            if (rutina == null || rutina.UsuarioId != usuarioId) return false;

            rutinaEjercicio.Series = dto.Series;
            rutinaEjercicio.Repeticiones = dto.Repeticiones;
            rutinaEjercicio.DescansoSegundos = dto.DescansoSegundos;
            rutinaEjercicio.PorcentajeDelPeso = dto.PorcentajeDelPeso;
            rutinaEjercicio.PesoSugerido = dto.PesoSugerido;

            await _repo.UpdateAsync(rutinaEjercicio);
            return true;
        }

        public async Task<bool> EliminarEjercicioDeRutinaAsync(int id, int usuarioId)
        {
            var rutinaEjercicio = await _repo.GetByIdAsync(id);
            if (rutinaEjercicio == null) return false;

            var rutina = await _rutinaRepo.GetByIdWithEjerciciosAsync(rutinaEjercicio.RutinaId);
            if (rutina == null || rutina.UsuarioId != usuarioId) return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
