using REPS_backend.DTOs.Progreso;
using REPS_backend.Models;
using REPS_backend.Repositories;
namespace REPS_backend.Services
{
    public class ProgresoService : IProgresoService
    {
        private readonly IEntrenamientoRepository _entrenamientoRepository;
        private readonly IRecordPersonalRepository _recordRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly Microsoft.Extensions.Logging.ILogger<ProgresoService> _logger;

        // Constantes de Puntos
        private const int PUNTOS_POR_SERIE = 10;
        private const int PUNTOS_POR_RECORD = 100;

        // Umbrales de Rango
        private const int UMBRAL_BRONCE = 500;
        private const int UMBRAL_PLATA = 1500;
        private const int UMBRAL_ORO = 3000;
        private const int UMBRAL_PLATINO = 5000;
        private const int UMBRAL_DIAMANTE = 8000;
        private const int UMBRAL_LEYENDA = 12000;

        public ProgresoService(
            IEntrenamientoRepository entrenamientoRepository, 
            IRecordPersonalRepository recordRepository,
            IUsuarioRepository usuarioRepository,
            Microsoft.Extensions.Logging.ILogger<ProgresoService> logger)
        {
            _entrenamientoRepository = entrenamientoRepository;
            _recordRepository = recordRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<List<ProgresoMuscularDto>> ObtenerProgresoMuscularAsync(int usuarioId)
        {
            try 
            {
                // 1. Obtener todo el historial
                var entrenamientos = await _entrenamientoRepository.GetByUsuarioIdWithSeriesAsync(usuarioId);

                // 2. Agrupar series por grupo muscular
                var todasLasSeries = entrenamientos?
                    .SelectMany(e => e.SeriesRealizadas ?? new List<SerieLog>())
                    .Where(s => s.Completada && s.Ejercicio != null)
                    .ToList() ?? new List<SerieLog>();

                // 3. Calcular puntos por grupo
                var puntosPorGrupo = todasLasSeries
                    .GroupBy(s => s.Ejercicio!.GrupoMuscular)
                    .Select(g => new
                    {
                        Grupo = g.Key,
                        Puntos = g.Count() * PUNTOS_POR_SERIE
                    })
                    .ToDictionary(k => k.Grupo, v => v.Puntos);

                // Agregar bonus por records personales
                var records = await _recordRepository.GetByUserIdAsync(usuarioId);
                foreach (var r in records.Where(rec => rec.Ejercicio != null))
                {
                    var grupo = r.Ejercicio!.GrupoMuscular;
                    if (!puntosPorGrupo.ContainsKey(grupo)) puntosPorGrupo[grupo] = 0;
                    puntosPorGrupo[grupo] += PUNTOS_POR_RECORD;
                }

                // 4. Contar entrenamientos por grupo
                var entrenosPorGrupo = new Dictionary<GrupoMuscular, int>();
                foreach (var e in entrenamientos ?? new List<Entrenamiento>())
                {
                    if (e.SeriesRealizadas == null) continue;

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

                // 5. Construir DTOs
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ObtenerProgresoMuscularAsync para usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<ProgresoGeneralDto> ObtenerProgresoGeneralAsync(int usuarioId)
        {
            var user = await _usuarioRepository.GetByIdAsync(usuarioId);

            int totalSinLogros = (user?.PuntosTotales ?? 0) - (user?.PuntosLogros ?? 0);

            return new ProgresoGeneralDto
            {
                PuntosTotales = totalSinLogros,
                RangoGeneral = user?.RangoGeneral.ToString() ?? "Bronce"
            };
        }

        private (string Rango, int SiguienteNivel, double Porcentaje) CalcularRango(int puntos)
        {
            if (puntos < UMBRAL_BRONCE)
                return ("Bronce", UMBRAL_BRONCE, (double)puntos / UMBRAL_BRONCE);

            if (puntos < UMBRAL_PLATA)
                return ("Plata", UMBRAL_PLATA, (double)(puntos - UMBRAL_BRONCE) / (UMBRAL_PLATA - UMBRAL_BRONCE));

            if (puntos < UMBRAL_ORO)
                return ("Oro", UMBRAL_ORO, (double)(puntos - UMBRAL_PLATA) / (UMBRAL_ORO - UMBRAL_PLATA));

            if (puntos < UMBRAL_PLATINO)
                return ("Platino", UMBRAL_PLATINO, (double)(puntos - UMBRAL_ORO) / (UMBRAL_PLATINO - UMBRAL_ORO));

            if (puntos < UMBRAL_DIAMANTE)
                return ("Diamante", UMBRAL_DIAMANTE, (double)(puntos - UMBRAL_PLATINO) / (UMBRAL_DIAMANTE - UMBRAL_PLATINO));

            if (puntos < UMBRAL_LEYENDA)
                return ("Leyenda", UMBRAL_LEYENDA, (double)(puntos - UMBRAL_DIAMANTE) / (UMBRAL_LEYENDA - UMBRAL_DIAMANTE));

            return ("Leyenda", puntos, 1.0); // Cap
        }

        public async Task<AnaliticaDto> ObtenerAnaliticaAsync(int usuarioId)
        {
            var entrenamientos = await _entrenamientoRepository.GetByUsuarioIdWithSeriesAsync(usuarioId);

            var pesosList = entrenamientos?
                .SelectMany(e => e.SeriesRealizadas ?? new List<SerieLog>())
                .Where(s => s.Peso > 0)
                .Select(s => (double)s.Peso)
                .TakeLast(7)
                .ToList() ?? new List<double>();

            if (!pesosList.Any()) pesosList = new List<double> { 0, 0, 0, 0, 0, 0, 0 };

            var volumenList = new List<double>();
            var actividad = new List<ActividadMensualDto>();
            var mesesTexto = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            
            for (int i = 0; i < 6; i++) {
                int monthIdx = DateTime.Now.Month - 5 + i;
                int yearOffset = 0;
                if (monthIdx <= 0) {
                    monthIdx += 12;
                    yearOffset = -1;
                }
                
                int month = monthIdx;
                int year = DateTime.Now.Year + yearOffset;
                
                int totalEntrenos = (entrenamientos ?? new List<Entrenamiento>()).Count(e => e.Fecha.Year == year && e.Fecha.Month == month);
                
                actividad.Add(new ActividadMensualDto {
                   Name = mesesTexto[month - 1],
                   Total = totalEntrenos,
                   Percent = totalEntrenos > 0 ? Math.Min(100, (totalEntrenos * 100) / 20) : 0
                });

                double volumenMes = entrenamientos?
                    .Where(e => e.Fecha.Year == year && e.Fecha.Month == month)
                    .SelectMany(e => e.SeriesRealizadas ?? new List<SerieLog>())
                    .Sum(s => (double)(s.Peso * s.Repeticiones)) ?? 0;
                    
                volumenList.Add(volumenMes);
            }

            return new AnaliticaDto
            {
                Pesos = pesosList,
                Volumen = volumenList,
                ActividadMensual = actividad
            };
        }
    }
}
