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

            // 1. Puntos de Logros
            var logros = await _logroRepository.GetUserLogrosAsync(userId);
            int puntosLogros = logros.Where(ul => ul.Desbloqueado).Sum(ul => ul.Logro.Puntos);
            usuario.PuntosLogros = puntosLogros;

            // 2. Calcular Rango Global basado en Grupos Musculares
            var records = await _recordRepository.GetByUserIdAsync(userId);
            
            // Agrupar records por Grupo Muscular y tomar el mejor levantamiento
            var bestLiftsByMuscle = records
                .GroupBy(r => r.Ejercicio.GrupoMuscular)
                .Select(g => new 
                { 
                    Grupo = g.Key, 
                    MaxPeso = g.Max(r => (double)r.PesoMaximo) 
                })
                .ToList();

            // Calcular rango para cada grupo muscular presente
            double sumRangoValues = 0;
            int totalGrupos = 10; // Hay 10 grupos en el Enum, o usamos Enum.GetValues(typeof(GrupoMuscular)).Length

            foreach (var lift in bestLiftsByMuscle)
            {
                var rangoMuscular = CalculateMuscleRank(lift.Grupo, lift.MaxPeso);
                sumRangoValues += (int)rangoMuscular;
            }

            // El promedio determina el rango global
            // Nota: Si no ha entrenado un músculo, es "SinRango" (0), lo que baja el promedio (es justo).
            double avgRango = sumRangoValues / totalGrupos;
            usuario.RangoGeneral = (Rango)Math.Round(avgRango);

            // 3. Actualizar Total
            // PuntosTotales = Logros + Records + (Bonus por Rango Global)
            // Bonus: Bronce=1000, Plata=2000...
            int bonusRango = (int)usuario.RangoGeneral * 1000; 
            usuario.PuntosTotales = usuario.PuntosLogros + usuario.PuntosRecords + bonusRango;

            await _usuarioRepository.UpdateUsuarioAsync(usuario);
        }

        private Rango CalculateMuscleRank(GrupoMuscular grupo, double peso)
        {
            // Lógica simplificada de umbrales.
            // En el futuro esto debería venir de una tabla en DB o config más compleja.
            
            // Umbrales base (ejemplo para ejercicios compuestos grandes)
            double baseBronce = 40;
            double basePlata = 80;
            double baseOro = 110;
            double baseDiamante = 140;
            double baseElite = 180;

            // Ajustes según grupo muscular (multiplicadores aproximados)
            double multiplier = 1.0;
            switch (grupo)
            {
                case GrupoMuscular.Pierna: multiplier = 1.5; break; // Sentadilla mueve más
                case GrupoMuscular.Espalda: multiplier = 1.2; break; // Peso muerto
                case GrupoMuscular.Pecho: multiplier = 1.0; break; // Bench press referencia
                case GrupoMuscular.Hombro: multiplier = 0.6; break;
                case GrupoMuscular.Biceps: 
                case GrupoMuscular.Triceps: multiplier = 0.4; break;
                case GrupoMuscular.Abdomen: multiplier = 0.0; break; // Difícil medir por peso
                default: multiplier = 0.5; break;
            }

            if (peso >= baseElite * multiplier) return Rango.Elite;
            if (peso >= baseDiamante * multiplier) return Rango.Diamante;
            if (peso >= baseOro * multiplier) return Rango.Oro;
            if (peso >= basePlata * multiplier) return Rango.Plata;
            if (peso >= baseBronce * multiplier) return Rango.Bronce;
            
            return Rango.SinRango;
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
                RachaDias = user.RachaDias,
                RangoGeneral = user.RangoGeneral.ToString(),
                RangosMusculares = new List<REPS_backend.DTOs.Ranking.MuscleProgressDto>()
            };

            var records = await _recordRepository.GetByUserIdAsync(userId);
            
            foreach (GrupoMuscular grupo in Enum.GetValues(typeof(GrupoMuscular)))
            {
                if (grupo == GrupoMuscular.Otro || grupo == GrupoMuscular.FullBody || grupo == GrupoMuscular.Cardio) continue;

                var bestLift = records
                    .Where(r => r.Ejercicio != null && r.Ejercicio.GrupoMuscular == grupo)
                    .OrderByDescending(r => r.PesoMaximo)
                    .FirstOrDefault();

                double currentWeight = bestLift != null ? (double)bestLift.PesoMaximo : 0;
                
                var progressDto = CalculateMuscleProgress(grupo, currentWeight);
                response.RangosMusculares.Add(progressDto);
            }

            return response;
        }

        private REPS_backend.DTOs.Ranking.MuscleProgressDto CalculateMuscleProgress(GrupoMuscular grupo, double peso)
        {
            double baseBronce = 40;
            double basePlata = 80;
            double baseOro = 110;
            double baseDiamante = 140;
            double baseElite = 180;

            double multiplier = 1.0;
            switch (grupo)
            {
                case GrupoMuscular.Pierna: multiplier = 1.5; break;
                case GrupoMuscular.Espalda: multiplier = 1.2; break;
                case GrupoMuscular.Pecho: multiplier = 1.0; break;
                case GrupoMuscular.Hombro: multiplier = 0.6; break;
                case GrupoMuscular.Biceps: 
                case GrupoMuscular.Triceps: multiplier = 0.4; break;
                case GrupoMuscular.Abdomen: multiplier = 0.0; break; 
                default: multiplier = 0.5; break;
            }

            double umbralBronce = baseBronce * multiplier;
            double umbralPlata = basePlata * multiplier;
            double umbralOro = baseOro * multiplier;
            double umbralDiamante = baseDiamante * multiplier;
            double umbralElite = baseElite * multiplier;

            string rangoActual = "Sin Rango";
            string nextRank = "Bronce";
            double nextThreshold = umbralBronce;
            double prevThreshold = 0;

            if (peso >= umbralElite) { rangoActual = "Elite"; nextRank = "Max"; nextThreshold = peso; prevThreshold = umbralElite; }
            else if (peso >= umbralDiamante) { rangoActual = "Diamante"; nextRank = "Elite"; nextThreshold = umbralElite; prevThreshold = umbralDiamante; }
            else if (peso >= umbralOro) { rangoActual = "Oro"; nextRank = "Diamante"; nextThreshold = umbralDiamante; prevThreshold = umbralOro; }
            else if (peso >= umbralPlata) { rangoActual = "Plata"; nextRank = "Oro"; nextThreshold = umbralOro; prevThreshold = umbralPlata; }
            else if (peso >= umbralBronce) { rangoActual = "Bronce"; nextRank = "Plata"; nextThreshold = umbralPlata; prevThreshold = umbralBronce; }

            double progressPct = 0;
            if (nextRank != "Max")
            {
                if (nextThreshold > prevThreshold)
                {
                    progressPct = (peso - prevThreshold) / (nextThreshold - prevThreshold) * 100;
                }
            }
            else
            {
                progressPct = 100;
            }
            
            progressPct = Math.Max(0, Math.Min(100, progressPct));

            return new REPS_backend.DTOs.Ranking.MuscleProgressDto
            {
                GrupoMuscular = grupo.ToString(),
                RangoActual = rangoActual,
                CurrentPoints = peso, 
                NextRankThreshold = nextThreshold,
                NextRank = nextRank,
                ProgressPercentage = Math.Round(progressPct, 1)
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
        }
    }
}
