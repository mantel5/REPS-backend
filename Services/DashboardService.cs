using REPS_backend.DTOs.Dashboard;
using REPS_backend.DTOs.Logros;
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
            try 
            {
                // 1. Datos básicos del usuario (Racha, Nivel)
                var perfil = await _usuarioService.ObtenerMiPerfilAsync(userId);
                if (perfil == null) throw new Exception("Usuario no encontrado");

                // 2. Últimos 3 logros
                List<LogroDTO> ultimosLogros = new();
                try {
                    ultimosLogros = await _logroService.GetUltimosLogrosDesbloqueadosAsync(userId, 3);
                } catch (Exception ex) {
                    Console.WriteLine($"Error cargando últimos logros: {ex.Message}");
                }

                // Total de logros
                int totalLogros = 0;
                int cantidadDesbloqueados = 0;
                try {
                    var todosLogros = await _logroService.GetAllAsync();
                    totalLogros = todosLogros.Count;

                    var logrosUser = await _logroService.GetLogrosForUserAsync(userId);
                    cantidadDesbloqueados = logrosUser.Count(l => l.Desbloqueado);
                } catch (Exception ex) {
                    Console.WriteLine($"Error calculando conteo de logros: {ex.Message}");
                }


                // 3. Siguiente Rutina (Lógica de carrusel)
                RutinaItemDto? rutinaSugerida = null;

                try {
                    // a) Obtener último entrenamiento con RutinaId
                    var historial = await _entrenamientoRepository.GetByUsuarioIdAsync(userId);
                    var ultimoEntrenoConRutina = historial?.FirstOrDefault(e => e.RutinaId.HasValue);

                    // b) Obtener todas las rutinas del usuario
                    var misRutinas = await _rutinaRepository.GetByUsuarioIdAsync(userId);
                    var rutinasOrdenadas = (misRutinas ?? new List<Rutina>()).OrderBy(r => r.Id).ToList();

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
                } catch (Exception ex) {
                    Console.WriteLine($"Error calculando rutina sugerida: {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR en DashboardService: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private RutinaItemDto MapRutinaToItemDto(Rutina r)
        {
            return new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Nivel = r.Nivel.ToString(),
                DuracionMinutos = r.DuracionMinutos,
                CreadorNombre = r.Usuario != null ? r.Usuario.Nombre : "Tú",
                Likes = r.Likes,
                CantidadEjercicios = r.Ejercicios?.Count ?? 0,
                TotalEjercicios = r.Ejercicios?.Count ?? 0
            };
        }
    }
}
