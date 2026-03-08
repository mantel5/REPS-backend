using REPS_backend.DTOs.Logros;
using REPS_backend.Models;
using REPS_backend.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REPS_backend.Services
{
    public class LogroService : ILogroService
    {
        private readonly ILogroRepository _logroRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEntrenamientoRepository _entrenamientoRepository;
        private readonly IRecordPersonalService _recordService;
        private readonly IRankingService _rankingService;

        public LogroService(
            ILogroRepository logroRepository, 
            IUsuarioRepository usuarioRepository, 
            IEntrenamientoRepository entrenamientoRepository, 
            IRecordPersonalService recordService,
            IRankingService rankingService)
        {
            _logroRepository = logroRepository;
            _usuarioRepository = usuarioRepository;
            _entrenamientoRepository = entrenamientoRepository;
            _recordService = recordService;
            _rankingService = rankingService;
        }

        public async Task<List<LogroDTO>> GetLogrosForUserAsync(int userId)
        {
            var user = await _usuarioRepository.GetByIdAsync(userId);
            var entrenamientosCount = await _entrenamientoRepository.CountByUsuarioIdAsync(userId);
            var recordsCount = (await _recordService.ObtenerRecordsUsuarioAsync(userId)).Count;
            var totalVolume = await _entrenamientoRepository.GetTotalVolumeByUsuarioIdAsync(userId); // Assume this method exists or add it

            var allLogros = await _logroRepository.GetAllAsync();
            var userLogros = await _logroRepository.GetUserLogrosAsync(userId);
            var userLogrosMap = userLogros
                .GroupBy(ul => ul.LogroId)
                .ToDictionary(g => g.Key, g => g.First());

            var result = new List<LogroDTO>();

            foreach (var logro in allLogros)
            {
                var dto = new LogroDTO
                {
                    Id = logro.Id,
                    Titulo = logro.Titulo,
                    Descripcion = logro.Descripcion,
                    Puntos = logro.Puntos,
                    IconoUrl = logro.IconoUrl,
                    Progreso = 0,
                    Desbloqueado = false
                };

                // Lógica dinámica avanzada para logros
                if (user != null)
                {
                    switch (logro.Titulo)
                    {
                        case "Primeros Pasos":
                            dto.Progreso = entrenamientosCount >= 1 ? 100 : 0;
                            break;
                        case "Centurión":
                        case "Guerrero del Hierro":
                            dto.Progreso = (int)Math.Min(100, (entrenamientosCount * 100.0) / 100);
                            break;
                        case "Racha de Fuego":
                            dto.Progreso = (int)Math.Min(100, (user.RachaDias * 100.0) / 28);
                            break;
                        case "Inquebrantable":
                            dto.Progreso = (int)Math.Min(100, (user.RachaDias * 100.0) / 60);
                            break;
                        case "Constancia":
                            // 3 sesiones semana. Difícil de calcular sin histórico completo, pero podemos aproximar.
                            dto.Progreso = entrenamientosCount >= 3 ? 100 : (int)(entrenamientosCount * 33.3);
                            break;
                        case "Estrella Ascendente":
                            // 50 entrenamientos / 30 días - aproximar con total
                            dto.Progreso = (int)Math.Min(100, (entrenamientosCount * 100.0) / 50);
                            break;
                        case "Velocista":
                            // 10 entrenamientos / semana - aproximar
                            dto.Progreso = (int)Math.Min(100, (entrenamientosCount * 100.0) / 10);
                            break;
                        case "Coleccionista de Records":
                            dto.Progreso = (int)Math.Min(100, (recordsCount * 100.0) / 5);
                            break;
                        case "Maestro del Volumen":
                            dto.Progreso = (int)Math.Min(100, (totalVolume * 100m) / 100000);
                            break;
                        case "Perfeccionista":
                            // 50 entrenamientos sin fallar - aproximar con total
                            dto.Progreso = (int)Math.Min(100, (entrenamientosCount * 100.0) / 50);
                            break;
                        default:
                            dto.Progreso = 0;
                            break;
                    }

                    if (dto.Progreso >= 100) 
                    {
                        // Don't set Desbloqueado here, let CheckAndUnlockAchievementsAsync handle it
                    }
                }

                if (userLogrosMap.TryGetValue(logro.Id, out var userLogro))
                {
                    dto.Progreso = Math.Max(dto.Progreso, userLogro.Progreso);
                    dto.Desbloqueado = dto.Desbloqueado || userLogro.Desbloqueado;
                    dto.FechaObtencion = userLogro.FechaObtencion;
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<LogroDTO> CreateLogroAsync(CreateLogroDTO dto)
        {
            var logro = new Logro
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Puntos = dto.Puntos,
                IconoUrl = dto.IconoUrl
            };

            var createdLogro = await _logroRepository.AddAsync(logro);

            return new LogroDTO
            {
                Id = createdLogro.Id,
                Titulo = createdLogro.Titulo,
                Descripcion = createdLogro.Descripcion,
                Puntos = createdLogro.Puntos,
                IconoUrl = createdLogro.IconoUrl,
                Progreso = 0,
                Desbloqueado = false
            };
        }

        public async Task<List<LogroDTO>> GetAllAsync()
        {
            var allLogros = await _logroRepository.GetAllAsync();
            return allLogros.Select(l => new LogroDTO
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Descripcion = l.Descripcion,
                Puntos = l.Puntos,
                IconoUrl = l.IconoUrl,
                Progreso = 0,
                Desbloqueado = false
            }).ToList();
        }

        public async Task<bool> UnlockLogroAsync(int userId, int logroId)
        {
            // 1. Verificar si ya lo tiene
            var userLogros = await _logroRepository.GetUserLogrosAsync(userId);
            if (userLogros.Any(ul => ul.LogroId == logroId && ul.Desbloqueado))
            {
                return false; // Ya lo tiene
            }

            // 2. Si existe el registro pero no desbloqueado, actualizarlo
            var existing = userLogros.FirstOrDefault(ul => ul.LogroId == logroId);
            if (existing != null)
            {
                existing.Desbloqueado = true;
                existing.FechaObtencion = System.DateTime.UtcNow;
                existing.Progreso = 100;
                await _logroRepository.UpdateUsuarioLogroAsync(existing);
            }
            else
            {
                var nuevoLogro = new UsuarioLogro
                {
                    UsuarioId = userId,
                    LogroId = logroId,
                    Desbloqueado = true,
                    FechaObtencion = System.DateTime.UtcNow,
                    Progreso = 100
                };
                await _logroRepository.AddUsuarioLogroAsync(nuevoLogro);
            }

            return true;
        }

        public async Task<List<LogroDTO>> CheckAndUnlockAchievementsAsync(int userId)
        {
            var allLogros = await GetLogrosForUserAsync(userId);
            var newlyUnlocked = new List<LogroDTO>();

            foreach (var logro in allLogros)
            {
                if (logro.Progreso >= 100 && !logro.Desbloqueado)
                {
                    // Unlock it AND add points
                    await UnlockLogroAndAddPointsAsync(userId, logro.Id);
                    logro.FechaObtencion = DateTime.UtcNow;
                    newlyUnlocked.Add(logro);
                }
            }

            return newlyUnlocked;
        }

        public async Task<List<LogroDTO>> GetUltimosLogrosDesbloqueadosAsync(int userId, int count)
        {
            var todosLogrosCalculados = await GetLogrosForUserAsync(userId);
            var desbloqueados = todosLogrosCalculados
                .Where(l => l.Desbloqueado)
                .OrderByDescending(l => l.FechaObtencion ?? DateTime.MinValue)
                .Take(count)
                .ToList();

            return desbloqueados;
        }

        public async Task<int> GetPuntosLogroAsync(int logroId)
        {
            var logro = await _logroRepository.GetByIdAsync(logroId);
            return logro?.Puntos ?? 0;
        }

        public async Task<bool> UnlockLogroAndAddPointsAsync(int userId, int logroId)
        {
            // 1. Verificar si ya lo tiene
            var userLogros = await _logroRepository.GetUserLogrosAsync(userId);
            if (userLogros.Any(ul => ul.LogroId == logroId && ul.Desbloqueado))
            {
                return false; // Ya lo tiene
            }

            // 2. Obtener los puntos del logro
            var logro = await _logroRepository.GetByIdAsync(logroId);
            if (logro == null) return false;

            int puntosLogro = logro.Puntos;

            // 3. Si existe el registro pero no desbloqueado, actualizarlo
            var existing = userLogros.FirstOrDefault(ul => ul.LogroId == logroId);
            if (existing != null)
            {
                existing.Desbloqueado = true;
                existing.FechaObtencion = System.DateTime.UtcNow;
                existing.Progreso = 100;
                await _logroRepository.UpdateUsuarioLogroAsync(existing);
            }
            else
            {
                var nuevoLogro = new UsuarioLogro
                {
                    UsuarioId = userId,
                    LogroId = logroId,
                    Desbloqueado = true,
                    FechaObtencion = System.DateTime.UtcNow,
                    Progreso = 100
                };
                await _logroRepository.AddUsuarioLogroAsync(nuevoLogro);
            }

            // 4. Actualizar los puntos del usuario
            var usuario = await _usuarioRepository.GetByIdAsync(userId);
            if (usuario != null)
            {
                usuario.PuntosLogros += puntosLogro;
                usuario.PuntosTotales += puntosLogro;
                await _usuarioRepository.UpdateUsuarioAsync(usuario);
                
                // 5. Actualizar el rango del usuario
                await _rankingService.UpdateUserRankAsync(userId);
            }

            return true;
        }
    }
}
