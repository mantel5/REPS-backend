using REPS_backend.DTOs.Dashboard;
using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;
using REPS_backend.Repositories;

namespace REPS_backend.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogroService _logroService;
        private readonly IEntrenamientoRepository _entrenamientoRepository;
        private readonly IRutinaRepository _rutinaRepository;

        public DashboardService(
            IUsuarioService usuarioService,
            ILogroService logroService,
            IEntrenamientoRepository entrenamientoRepository,
            IRutinaRepository rutinaRepository)
        {
            _usuarioService = usuarioService;
            _logroService = logroService;
            _entrenamientoRepository = entrenamientoRepository;
            _rutinaRepository = rutinaRepository;
        }

        public async Task<DashboardResumenDto> ObtenerResumenHomeAsync(int userId)
        {
            // 1. Datos básicos del usuario (Racha, Nivel)
            var perfil = await _usuarioService.ObtenerMiPerfilAsync(userId);
            if (perfil == null) throw new Exception("Usuario no encontrado");

            // 2. Últimos 3 logros
            var ultimosLogros = await _logroService.GetUltimosLogrosDesbloqueadosAsync(userId, 3);

            // Total de logros (para mostrar X/Y logros desbloqueados)
            var todosLogros = await _logroService.GetAllAsync();
            int totalLogros = todosLogros.Count;
            // int desbloqueadosCount = perfil.PuntosLogros; // Eliminado por no existir y no usarse bien

            var logrosUser = await _logroService.GetLogrosForUserAsync(userId);
            int cantidadDesbloqueados = logrosUser.Count(l => l.Desbloqueado);


            // 3. Siguiente Rutina (Lógica de carrusel)
            RutinaItemDto? rutinaSugerida = null;

            // a) Obtener último entrenamiento con RutinaId
            var historial = await _entrenamientoRepository.GetByUsuarioIdAsync(userId);
            // Asumimos que GetByUsuarioIdAsync devuelve ordenado por fecha desc
            // Si no, habría que ordenar.
            // Necesitamos que Entrenamientos tenga RutinaId poblado.

            var ultimoEntrenoConRutina = historial.FirstOrDefault(e => e.RutinaId.HasValue);

            // b) Obtener todas las rutinas del usuario (o públicas favoritas, por ahora "Mis Rutinas")
            var misRutinas = await _rutinaRepository.GetByUsuarioIdAsync(userId);
            var rutinasOrdenadas = misRutinas.OrderBy(r => r.Id).ToList();

            if (rutinasOrdenadas.Any())
            {
                if (ultimoEntrenoConRutina != null)
                {
                    // Buscar índice
                    int index = rutinasOrdenadas.FindIndex(r => r.Id == ultimoEntrenoConRutina.RutinaId);

                    if (index != -1)
                    {
                        // Siguiente
                        int nextIndex = (index + 1) % rutinasOrdenadas.Count;
                        var siguienteRutina = rutinasOrdenadas[nextIndex];
                        rutinaSugerida = MapRutinaToItemDto(siguienteRutina);
                    }
                    else
                    {
                        // Si la rutina del último entreno ya no existe, sugerir la primera
                        rutinaSugerida = MapRutinaToItemDto(rutinasOrdenadas.First());
                    }
                }
                else
                {
                    // Nunca ha entrenado con rutina -> Sugerir la primera
                    rutinaSugerida = MapRutinaToItemDto(rutinasOrdenadas.First());
                }
            }

            return new DashboardResumenDto
            {
                RachaDias = perfil.RachaDias,
                Nivel = perfil.RangoGeneral,
                LogrosDesbloqueados = cantidadDesbloqueados,
                TotalLogros = totalLogros,
                UltimosLogros = ultimosLogros,
                RutinaSugerida = rutinaSugerida
            };
        }

        private RutinaItemDto MapRutinaToItemDto(Rutina r)
        {
            return new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                CreadorNombre = r.Usuario != null ? r.Usuario.Nombre : "Tú",
                Likes = r.Likes,
                TotalEjercicios = r.Ejercicios?.Count ?? 0
            };
        }
    }
}
