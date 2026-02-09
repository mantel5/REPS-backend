using REPS_backend.DTOs.Progreso;
using REPS_backend.Models;
using REPS_backend.Repositories;

namespace REPS_backend.Services
{
    public class ProgresoService : IProgresoService
    {
        private readonly IEntrenamientoRepository _entrenamientoRepository;

        // Constantes de Puntos
        private const int PUNTOS_POR_SERIE = 10;

        // Umbrales de Rango
        private const int UMBRAL_BRONCE = 500;
        private const int UMBRAL_PLATA = 1500;
        private const int UMBRAL_ORO = 3000;
        private const int UMBRAL_PLATINO = 5000;
        private const int UMBRAL_DIAMANTE = 8000;
        private const int UMBRAL_LEYENDA = 12000;

        public ProgresoService(IEntrenamientoRepository entrenamientoRepository)
        {
            _entrenamientoRepository = entrenamientoRepository;
        }

        public async Task<List<ProgresoMuscularDto>> ObtenerProgresoMuscularAsync(int usuarioId)
        {
            // 1. Obtener todo el historial
            var entrenamientos = await _entrenamientoRepository.GetByUsuarioIdWithSeriesAsync(usuarioId);

            // 2. Agrupar series por grupo muscular
            // Necesitamos 'aplanar' la lista de series
            var todasLasSeries = entrenamientos
                .SelectMany(e => e.SeriesRealizadas)
                .Where(s => s.Completada && s.Ejercicio != null)
                .ToList();

            // 3. Calcular puntos por grupo
            var puntosPorGrupo = todasLasSeries
                .GroupBy(s => s.Ejercicio!.GrupoMuscular)
                .Select(g => new
                {
                    Grupo = g.Key,
                    Puntos = g.Count() * PUNTOS_POR_SERIE
                })
                .ToDictionary(k => k.Grupo, v => v.Puntos);

            // 4. Contar entrenamientos por grupo (si al menos 1 ejercicio del grupo se hizo en la sesión)
            var entrenosPorGrupo = new Dictionary<GrupoMuscular, int>();
            foreach (var e in entrenamientos)
            {
                var gruposEnEsteEntreno = e.SeriesRealizadas
                    .Where(s => s.Completada && s.Ejercicio != null)
                    .Select(s => s.Ejercicio!.GrupoMuscular)
                    .Distinct();

                foreach (var g in gruposEnEsteEntreno)
                {
                    if (!entrenosPorGrupo.ContainsKey(g)) entrenosPorGrupo[g] = 0;
                    entrenosPorGrupo[g]++;
                }
            }

            // 5. Construir DTOs para TODOS los grupos musculares (incluso con 0 puntos)
            var resultado = new List<ProgresoMuscularDto>();
            foreach (GrupoMuscular grupo in Enum.GetValues(typeof(GrupoMuscular)))
            {
                int puntos = puntosPorGrupo.ContainsKey(grupo) ? puntosPorGrupo[grupo] : 0;
                int countEntrenos = entrenosPorGrupo.ContainsKey(grupo) ? entrenosPorGrupo[grupo] : 0;

                var infoRango = CalcularRango(puntos);

                resultado.Add(new ProgresoMuscularDto
                {
                    GrupoMuscular = grupo.ToString(),
                    Rango = infoRango.Rango,
                    PuntosActuales = puntos,
                    SiguienteNivelPuntos = infoRango.SiguienteNivel,
                    PuntosParaSiguienteNivel = infoRango.SiguienteNivel - puntos,
                    Porcentaje = infoRango.Porcentaje,
                    EntrenamientosRealizados = countEntrenos
                });
            }

            return resultado.OrderByDescending(r => r.PuntosActuales).ToList();
        }

        public async Task<ProgresoGeneralDto> ObtenerProgresoGeneralAsync(int usuarioId)
        {
            // Podríamos sumar todos los puntos de todos los grupos, o calcularlo aparte.
            // Por simplicidad, sumaremos los puntos de series completadas.
            var entrenamientos = await _entrenamientoRepository.GetByUsuarioIdWithSeriesAsync(usuarioId);

            int totalSeries = entrenamientos
               .SelectMany(e => e.SeriesRealizadas)
               .Count(s => s.Completada);

            int puntosTotales = totalSeries * PUNTOS_POR_SERIE;
            var infoRango = CalcularRango(puntosTotales); // Usamos la misma escala? O una x10? 
                                                          // Asumiremos misma escala para simplificar por ahora, o x5. 
                                                          // En realidad el rango general suele ser mas alto. Multiplicaré umbrales por 5 mentalmente o uso escala dedicada.
                                                          // Para esta iteración, uso la misma lógica de Rangos pero "General" podría requerir más puntos.
                                                          // Dejemoslo igual por ahora.

            return new ProgresoGeneralDto
            {
                PuntosTotales = puntosTotales,
                RangoGeneral = infoRango.Rango
            };
        }

        private (string Rango, int SiguienteNivel, double Porcentaje) CalcularRango(int puntos)
        {
            if (puntos < UMBRAL_BRONCE)
                return ("SinRango", UMBRAL_BRONCE, (double)puntos / UMBRAL_BRONCE);

            if (puntos < UMBRAL_PLATA)
                return ("Bronce", UMBRAL_PLATA, (double)(puntos - UMBRAL_BRONCE) / (UMBRAL_PLATA - UMBRAL_BRONCE));

            if (puntos < UMBRAL_ORO)
                return ("Plata", UMBRAL_ORO, (double)(puntos - UMBRAL_PLATA) / (UMBRAL_ORO - UMBRAL_PLATA));

            if (puntos < UMBRAL_PLATINO)
                return ("Oro", UMBRAL_PLATINO, (double)(puntos - UMBRAL_ORO) / (UMBRAL_PLATINO - UMBRAL_ORO));

            if (puntos < UMBRAL_DIAMANTE)
                return ("Platino", UMBRAL_DIAMANTE, (double)(puntos - UMBRAL_PLATINO) / (UMBRAL_DIAMANTE - UMBRAL_PLATINO));

            if (puntos < UMBRAL_LEYENDA)
                return ("Diamante", UMBRAL_LEYENDA, (double)(puntos - UMBRAL_DIAMANTE) / (UMBRAL_LEYENDA - UMBRAL_DIAMANTE));

            return ("Leyenda", puntos, 1.0); // Cap
        }
    }
}
