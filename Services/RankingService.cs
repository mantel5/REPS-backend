using REPS_backend.Models;
using REPS_backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REPS_backend.Services
{
    public interface IRankingService
    {
        Task UpdateUserRankAsync(int userId);
        Task AddRecordPointsAsync(int userId, int points);
        Task<List<REPS_backend.DTOs.Ranking.LeaderboardItemDto>> GetLeaderboardAsync();
        Task<REPS_backend.DTOs.Ranking.UserProgressResponseDto?> GetUserProgressAsync(int userId);
        Task UpdateStreakAsync(int userId);
    }

    public class RankingService : IRankingService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogroRepository _logroRepository;
        private readonly IRecordPersonalRepository _recordRepository;
        private readonly IEntrenamientoRepository _entrenamientoRepository;

        public RankingService(
            IUsuarioRepository usuarioRepository, 
            ILogroRepository logroRepository,
            IRecordPersonalRepository recordRepository,
            IEntrenamientoRepository entrenamientoRepository)
        {
            _usuarioRepository = usuarioRepository;
            _logroRepository = logroRepository;
            _recordRepository = recordRepository;
            _entrenamientoRepository = entrenamientoRepository;
        }

        public async Task AddRecordPointsAsync(int userId, int points)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(userId);
            if (usuario == null) return;

            usuario.PuntosRecords += points;
            await _usuarioRepository.UpdateUsuarioAsync(usuario);
            
            // Recalcular puntos totales y ranks
            await UpdateUserRankAsync(userId);
        }

        public async Task UpdateUserRankAsync(int userId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(userId);
            if (usuario == null) return;

            // Skip recalculation for test user hola
            if (usuario.Email == "hola@gmail.com") return;

            // 1. Puntos de Logros
            var logros = await _logroRepository.GetUserLogrosAsync(userId);
            int puntosLogros = logros.Where(ul => ul.Desbloqueado).Sum(ul => ul.Logro.Puntos);
            usuario.PuntosLogros = puntosLogros;

            // 2. Calcular Puntos por Músculo
            var records = await _recordRepository.GetByUserIdAsync(userId);
            var entrenamientos = await _entrenamientoRepository.GetByUsuarioIdWithSeriesAsync(userId);
            
            var seriesCompletadas = entrenamientos?
                .SelectMany(e => e.SeriesRealizadas ?? new List<SerieLog>())
                .Where(s => s.Completada && s.Ejercicio != null)
                .ToList() ?? new List<SerieLog>();

            var puntosPorMusculo = new Dictionary<GrupoMuscular, int>();
            
            foreach (var s in seriesCompletadas)
            {
                var grupo = s.Ejercicio!.GrupoMuscular;
                if (!puntosPorMusculo.ContainsKey(grupo)) puntosPorMusculo[grupo] = 0;
                puntosPorMusculo[grupo] += 10;
            }

            foreach(var r in records.Where(r => r.Ejercicio != null))
            {
                var grupo = r.Ejercicio!.GrupoMuscular;
                if (!puntosPorMusculo.ContainsKey(grupo)) puntosPorMusculo[grupo] = 0;
                puntosPorMusculo[grupo] += 100;
            }

            var gruposValidos = Enum.GetValues(typeof(GrupoMuscular)).Cast<GrupoMuscular>()
                   .Where(g => g != GrupoMuscular.Otro && g != GrupoMuscular.FullBody && g != GrupoMuscular.Cardio).ToList();
            
            double sumPuntosMusculos = 0;
            foreach(var grupo in gruposValidos)
            {
               if(puntosPorMusculo.ContainsKey(grupo))
                  sumPuntosMusculos += puntosPorMusculo[grupo];
            }

            // 3. Actualizar Total: Puntos del Rango General (Suma Muscular) + Puntos de Logros
            int puntosRangoGeneral = (int)Math.Round(sumPuntosMusculos);
            int currentPuntosRango = usuario.PuntosTotales - usuario.PuntosLogros;

            if (puntosRangoGeneral > currentPuntosRango)
            {
                usuario.PuntosTotales = puntosRangoGeneral + usuario.PuntosLogros;
            }

            // Determinar enum RangoGeneral based on the higher of calculated or current
            int effectivePuntosRango = Math.Max(puntosRangoGeneral, currentPuntosRango);
            usuario.RangoGeneral = ConvertPuntosARango(effectivePuntosRango);

            await _usuarioRepository.UpdateUsuarioAsync(usuario);

            // Check for achievements after updating rank
            // await _logroService.CheckAndUnlockAchievementsAsync(userId);
        }

        private Rango ConvertPuntosARango(int puntos)
        {
            if (puntos < 2000) return Rango.Bronce;
            if (puntos < 3000) return Rango.Plata;
            if (puntos < 4000) return Rango.Oro;
            if (puntos < 5000) return Rango.Platino;
            if (puntos < 6000) return Rango.Diamante;
            if (puntos < 7000) return Rango.Leyenda;
            if (puntos < 8000) return Rango.Maestro;
            if (puntos < 9000) return Rango.GranMaestro;
            if (puntos < 10000) return Rango.Challenger;
            if (puntos < 11000) return Rango.Mitico;
            return Rango.Legendario;
        }

        public async Task<List<REPS_backend.DTOs.Ranking.LeaderboardItemDto>> GetLeaderboardAsync()
        {
            var users = await _usuarioRepository.GetAllAsync();
            
            var topUsers = users
                .OrderByDescending(u => u.PuntosTotales)
                .Take(50)
                .ToList();

            var leaderboard = new List<REPS_backend.DTOs.Ranking.LeaderboardItemDto>();
            int index = 1;

            foreach (var u in topUsers)
            {
                int totalWorkouts = await _entrenamientoRepository.CountByUsuarioIdAsync(u.Id);
                leaderboard.Add(new REPS_backend.DTOs.Ranking.LeaderboardItemDto
                {
                    Posicion = index++,
                    CambioPosicion = 0,
                    UsuarioId = u.Id,
                    Nombre = u.Nombre,
                    AvatarId = u.AvatarId,
                    PuntosTotales = u.PuntosTotales,
                    PuntosRangoGeneral = u.PuntosTotales - u.PuntosLogros, // Reversing the sum or calculating it
                    EntrenamientosTotal = totalWorkouts,
                    RangoGeneral = u.RangoGeneral.ToString()
                });
            }

            return leaderboard;
        }

        public async Task<REPS_backend.DTOs.Ranking.UserProgressResponseDto?> GetUserProgressAsync(int userId)
        {
            var user = await _usuarioRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var response = new REPS_backend.DTOs.Ranking.UserProgressResponseDto
            {
                PuntosTotales = user.PuntosTotales,
                PuntosRangoGeneral = user.PuntosTotales - user.PuntosLogros,
                RachaDias = user.RachaDias,
                RangoGeneral = user.RangoGeneral.ToString(),
                RangosMusculares = new List<REPS_backend.DTOs.Ranking.MuscleProgressDto>()
            };

            var records = await _recordRepository.GetByUserIdAsync(userId);
            var entrenamientos = await _entrenamientoRepository.GetByUsuarioIdWithSeriesAsync(userId);
            var seriesCompletadas = entrenamientos?
                .SelectMany(e => e.SeriesRealizadas ?? new List<SerieLog>())
                .Where(s => s.Completada && s.Ejercicio != null)
                .ToList() ?? new List<SerieLog>();
            
            var puntosPorMusculo = new Dictionary<GrupoMuscular, int>();
            foreach (var s in seriesCompletadas)
            {
                var grupo = s.Ejercicio!.GrupoMuscular;
                if (!puntosPorMusculo.ContainsKey(grupo)) puntosPorMusculo[grupo] = 0;
                puntosPorMusculo[grupo] += 10;
            }

            foreach(var r in records.Where(r => r.Ejercicio != null))
            {
                var grupo = r.Ejercicio!.GrupoMuscular;
                if (!puntosPorMusculo.ContainsKey(grupo)) puntosPorMusculo[grupo] = 0;
                puntosPorMusculo[grupo] += 100;
            }

            foreach (GrupoMuscular grupo in Enum.GetValues(typeof(GrupoMuscular)))
            {
                if (grupo == GrupoMuscular.Otro || grupo == GrupoMuscular.FullBody || grupo == GrupoMuscular.Cardio) continue;

                int puntos = puntosPorMusculo.ContainsKey(grupo) ? puntosPorMusculo[grupo] : 0;
                var progressDto = CalculateMuscleProgressFromPoints(grupo, puntos);
                response.RangosMusculares.Add(progressDto);
            }

            return response;
        }

        private REPS_backend.DTOs.Ranking.MuscleProgressDto CalculateMuscleProgressFromPoints(GrupoMuscular grupo, int puntos)
        {
            double baseBronce = 500;
            double basePlata = 2000;
            double baseOro = 3000;
            double basePlatino = 4000;
            double baseDiamante = 5000;
            double baseLeyenda = 6000;
            double baseMaestro = 7000;
            double baseGranMaestro = 8000;
            double baseChallenger = 9000;
            double baseMitico = 10000;
            double baseLegendario = 11000;

            string rangoActual = "Bronce";
            string nextRank = "Plata";
            double nextThreshold = baseBronce;
            double prevThreshold = 0;

            if (puntos >= baseLegendario) { rangoActual = "Legendario"; nextRank = "Max"; nextThreshold = puntos; prevThreshold = baseLegendario; }
            else if (puntos >= baseMitico) { rangoActual = "Mítico"; nextRank = "Legendario"; nextThreshold = baseLegendario; prevThreshold = baseMitico; }
            else if (puntos >= baseChallenger) { rangoActual = "Challenger"; nextRank = "Mítico"; nextThreshold = baseMitico; prevThreshold = baseChallenger; }
            else if (puntos >= baseGranMaestro) { rangoActual = "Gran Maestro"; nextRank = "Challenger"; nextThreshold = baseChallenger; prevThreshold = baseGranMaestro; }
            else if (puntos >= baseMaestro) { rangoActual = "Maestro"; nextRank = "Gran Maestro"; nextThreshold = baseGranMaestro; prevThreshold = baseMaestro; }
            else if (puntos >= baseLeyenda) { rangoActual = "Leyenda"; nextRank = "Maestro"; nextThreshold = baseMaestro; prevThreshold = baseLeyenda; }
            else if (puntos >= baseDiamante) { rangoActual = "Diamante"; nextRank = "Leyenda"; nextThreshold = baseLeyenda; prevThreshold = baseDiamante; }
            else if (puntos >= basePlatino) { rangoActual = "Platino"; nextRank = "Diamante"; nextThreshold = baseDiamante; prevThreshold = basePlatino; }
            else if (puntos >= baseOro) { rangoActual = "Oro"; nextRank = "Platino"; nextThreshold = basePlatino; prevThreshold = baseOro; }
            else if (puntos >= basePlata) { rangoActual = "Plata"; nextRank = "Oro"; nextThreshold = baseOro; prevThreshold = basePlata; }
            else if (puntos >= baseBronce) { rangoActual = "Bronce"; nextRank = "Plata"; nextThreshold = basePlata; prevThreshold = baseBronce; }
            else { rangoActual = "Sin Rango"; nextRank = "Bronce"; nextThreshold = baseBronce; prevThreshold = 0; }

            double progressPct = 0;
            if (nextRank != "Max" && nextThreshold > prevThreshold)
            {
                progressPct = (double)(puntos - prevThreshold) / (nextThreshold - prevThreshold) * 100.0;
            }
            else if (nextRank == "Max") progressPct = 100;

            return new REPS_backend.DTOs.Ranking.MuscleProgressDto
            {
                GrupoMuscular = grupo.ToString(),
                RangoActual = rangoActual,
                CurrentPoints = puntos, 
                NextRankThreshold = nextThreshold,
                NextRank = nextRank,
                ProgressPercentage = Math.Round(Math.Max(0, Math.Min(100, progressPct)), 1)
            };
        }

        public async Task UpdateStreakAsync(int userId)
        {
            var user = await _usuarioRepository.GetByIdAsync(userId);
            if (user == null) return;

            var today = DateTime.UtcNow.Date;
            var lastActivity = user.FechaUltimaActividad?.Date;

            if (lastActivity == today)
            {
                // Ya tuvo actividad hoy, no hacemos nada
                return;
            }

            if (lastActivity == today.AddDays(-1))
            {
                // Actividad ayer, incrementamos racha
                user.RachaDias++;
            }
            else
            {
                // Perdió la racha (o es la primera vez)
                user.RachaDias = 1;
            }

            user.FechaUltimaActividad = DateTime.UtcNow;
            await _usuarioRepository.UpdateUsuarioAsync(user);

            // Check for achievements after updating streak
            // await _logroService.CheckAndUnlockAchievementsAsync(userId);
        }
    }
}
