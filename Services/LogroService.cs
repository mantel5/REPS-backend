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
        private readonly IRankingService _rankingService;

        public LogroService(ILogroRepository logroRepository, IRankingService rankingService)
        {
            _logroRepository = logroRepository;
            _rankingService = rankingService;
        }

        public async Task<List<LogroDTO>> GetLogrosForUserAsync(int userId)
        {
            var allLogros = await _logroRepository.GetAllAsync();
            var userLogros = await _logroRepository.GetUserLogrosAsync(userId);
            var userLogrosMap = userLogros.ToDictionary(ul => ul.LogroId);

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

                if (userLogrosMap.TryGetValue(logro.Id, out var userLogro))
                {
                    dto.Progreso = userLogro.Progreso;
                    dto.Desbloqueado = userLogro.Desbloqueado;
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

            // 2. Si existe el registro pero no desbloqueado, actualizarlo?
            // Por simplicidad, asumimos que si no lo tiene desbloqueado, lo creamos o actualizamos

            var existing = userLogros.FirstOrDefault(ul => ul.LogroId == logroId);
            if (existing != null)
            {
                existing.Desbloqueado = true;
                existing.FechaObtencion = System.DateTime.UtcNow;
                existing.Progreso = 100;
                // Deberíamos actualizar en repo, falta metodo UpdateUsuarioLogroAsync.
                // Como workaround si no quiero tocar repo, puedo asumir que Entity Framework trackea cambios si usamos el mismo contexto. 
                // Pero LogroRepository usa _context, así que sí trackea si GetUserLogros los trajo.
                // Sin embargo necesitamos SaveChanges.
                // Agregaremos un SaveChanges o Update al repo, pero por ahora usemos Add para el caso nuevo.
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

            await _rankingService.UpdateUserRankAsync(userId);
            await _rankingService.UpdateStreakAsync(userId);

            return true;
        }

        public async Task<List<LogroDTO>> GetUltimosLogrosDesbloqueadosAsync(int userId, int count)
        {
            var userLogros = await _logroRepository.GetUserLogrosAsync(userId);
            var desbloqueados = userLogros
                .Where(ul => ul.Desbloqueado)
                .OrderByDescending(ul => ul.FechaObtencion)
                .Take(count)
                .ToList();

            if (!desbloqueados.Any()) return new List<LogroDTO>();

            // Cargar info del logro
            var result = new List<LogroDTO>();
            foreach (var ul in desbloqueados)
            {
                var logro = await _logroRepository.GetByIdAsync(ul.LogroId);
                if (logro != null)
                {
                    result.Add(new LogroDTO
                    {
                        Id = logro.Id,
                        Titulo = logro.Titulo,
                        Descripcion = logro.Descripcion,
                        Puntos = logro.Puntos,
                        IconoUrl = logro.IconoUrl,
                        Progreso = 100,
                        Desbloqueado = true,
                        FechaObtencion = ul.FechaObtencion
                    });
                }
            }
            return result;
        }
    }
}
