using REPS_backend.DTOs.Rutinas;
using REPS_backend.Models;
using REPS_backend.Repositories; // <-- Importamos Repositories

namespace REPS_backend.Services
{
    public class RutinaService : IRutinaService
    {
        private readonly IRutinaRepository _repository;

        public RutinaService(IRutinaRepository repository)
        {
            _repository = repository;
        }

        public async Task<RutinaDetalleDto> CrearRutinaAsync(RutinaCreateDto dto, int usuarioId)
        {
            // 1. Mapear DTO a Entidad
            var nuevaRutina = new Rutina
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                Estado = EstadoRutina.Privada,
                Ejercicios = new List<RutinaEjercicio>()
            };

            // 2. Lógica interna (Smart Weight y Ejercicios)
            foreach (var ejDto in dto.Ejercicios)
            {
                var ejercicioDominio = new RutinaEjercicio
                {
                    Series = ejDto.Series,
                    DescansoSegundos = ejDto.DescansoSegundos,
                    Tipo = ejDto.Tipo,
                    PorcentajeDelPeso = CalcularPorcentajeSmart(ejDto.Tipo),
                    PesoSugerido = 0
                };
                nuevaRutina.Ejercicios.Add(ejercicioDominio);
            }

            // 3. Calcular duración
            nuevaRutina.DuracionMinutos = CalcularDuracionInterna(nuevaRutina.Ejercicios);

            // 4. USAR EL REPOSITORIO (Como en tu proyecto anterior)
            await _repository.AddAsync(nuevaRutina); 
            
            // 5. Devolver DTO
            return MapToDetalleDto(nuevaRutina);
        }

        public async Task<List<RutinaItemDto>> ObtenerRutinasPublicasAsync()
        {
            // Pedimos los datos al repositorio real
            var rutinas = await _repository.GetAllPublicasAsync();

            // Convertimos a DTO
            return rutinas.Select(r => new RutinaItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Nivel = r.Nivel.ToString(),
                DuracionMinutos = r.DuracionMinutos,
                CantidadEjercicios = r.Ejercicios.Count
            }).ToList();
        }

        public async Task<RutinaDetalleDto> ObtenerDetalleRutinaAsync(int rutinaId)
        {
            var rutina = await _repository.GetByIdAsync(rutinaId);

            if (rutina == null) return null;

            return MapToDetalleDto(rutina);
        }

        // --- MÉTODOS PRIVADOS (Los mismos de antes) ---
        private decimal CalcularPorcentajeSmart(TipoSerie tipo)
        {
             return tipo switch
            {
                TipoSerie.Calentamiento => 0.50m,
                TipoSerie.Aproximacion => 0.75m, 
                TipoSerie.DropSet => 0.60m,      
                TipoSerie.AlFallo => 0.85m,      
                _ => 1.0m                        
            };
        }

        private int CalcularDuracionInterna(List<RutinaEjercicio> ejercicios)
        {
            if (ejercicios == null || !ejercicios.Any()) return 0;
            double segundosTotales = 0;
            foreach (var ej in ejercicios)
            {
                segundosTotales += (ej.Series * 60);
                if (ej.Series > 1) segundosTotales += (ej.Series - 1) * ej.DescansoSegundos;
            }
            segundosTotales += (ejercicios.Count * 120);
            return (int)Math.Ceiling(segundosTotales / 60);
        }

        private RutinaDetalleDto MapToDetalleDto(Rutina r)
        {
            return new RutinaDetalleDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Nivel = r.Nivel.ToString(),
                DuracionMinutos = r.DuracionMinutos,
                Estado = r.Estado.ToString(),
                Ejercicios = r.Ejercicios?.Select(e => new RutinaEjercicioDto
                {
                    EjercicioId = 0, 
                    Series = e.Series,
                    DescansoSegundos = e.DescansoSegundos,
                    Tipo = e.Tipo
                }).ToList() ?? new List<RutinaEjercicioDto>()
            };
        }
    }
}