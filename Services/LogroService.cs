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

        public LogroService(ILogroRepository logroRepository)
        {
            _logroRepository = logroRepository;
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
    }
}
