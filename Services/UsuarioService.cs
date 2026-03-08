using REPS_backend.DTOs.Usuarios;
using REPS_backend.Repositories;
using REPS_backend.Models;

namespace REPS_backend.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IRankingService _rankingService;

        public UsuarioService(IUsuarioRepository repository, IRankingService rankingService)
        {
            _repository = repository;
            _rankingService = rankingService;
        }

        // ... MÉTODOS EXISTENTES (MiPerfil, BuscarAmigo, Actualizar) ...
        public async Task<UsuarioPerfilDto?> ObtenerMiPerfilAsync(int id)
        {
            // Forzamos recalcular rangos por si los umbrales han cambiado
            await _rankingService.UpdateUserRankAsync(id);

            var user = await _repository.GetByIdAsync(id);
            if (user == null || !user.EstaActivo || user.EstaBorrado) return null;

            return new UsuarioPerfilDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                AvatarId = user.AvatarId,
                CodigoAmigo = user.CodigoAmigo,
                FechaRegistro = user.FechaRegistro,
                Rol = user.Rol,
                PuntosTotales = user.PuntosTotales,
                PuntosRangoGeneral = user.PuntosTotales - user.PuntosLogros,
                PuntosLogros = user.PuntosLogros,
                RachaDias = user.RachaDias,
                RangoGeneral = user.RangoGeneral.ToString(),
                Biografia = user.Biografia,
                EsPerfilPublico = user.EsPerfilPublico,
                MostrarEstadisticas = user.MostrarEstadisticas,
                RankingVisible = user.RankingVisible,
                EsPro = user.EsPro()
            };
        }

        public async Task<UsuarioPublicoDto?> BuscarAmigoPorCodigoAsync(string codigo)
        {
            var user = await _repository.GetByCodigoAmigoAsync(codigo.ToUpper());
            if (user == null) return null;

            return new UsuarioPublicoDto
            {
                Nombre = user.Nombre,
                AvatarId = user.AvatarId,
                FechaRegistro = user.FechaRegistro,
                PuntosTotales = user.PuntosTotales,
                RachaDias = user.RachaDias,
                RangoGeneral = user.RangoGeneral.ToString(),
                EsPro = user.EsPro(),
                Biografia = user.Biografia,
                CodigoAmigo = user.CodigoAmigo
            };
        }

        public async Task<bool> ActualizarPerfilAsync(int id, UsuarioUpdateDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            if (dto.Nombre != null) user.Nombre = dto.Nombre;
            if (dto.AvatarId != null) user.AvatarId = dto.AvatarId;
            if (dto.Biografia != null) user.Biografia = dto.Biografia;
            
            if (dto.EsPerfilPublico.HasValue) user.EsPerfilPublico = dto.EsPerfilPublico.Value;
            if (dto.MostrarEstadisticas.HasValue) user.MostrarEstadisticas = dto.MostrarEstadisticas.Value;
            if (dto.RankingVisible.HasValue) user.RankingVisible = dto.RankingVisible.Value;

            await _repository.UpdateUsuarioAsync(user);
            return true;
        }

        // LÓGICA DE ADMINISTRADOR

        public async Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAdminAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> CambiarEstadoBloqueoAsync(int id, bool nuevoEstadoActivo)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            user.EstaActivo = nuevoEstadoActivo;
            
            await _repository.UpdateUsuarioAsync(user);
            return true;
        }

        public async Task<bool> EliminarUsuarioLogicoAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            user.EstaBorrado = true;
            user.EstaActivo = false; 

            await _repository.UpdateUsuarioAsync(user);
            return true;
        }

        public async Task<bool> AgregarAmigoAsync(int miId, string codigoAmigo)
        {
            // 1. Buscamos al futuro amigo
            var amigo = await _repository.GetByCodigoAmigoAsync(codigoAmigo.ToUpper());
            
            // VALIDACIONES:
            // - Si no existe
            // - O si soy yo mismo (mi ID es igual al del amigo)
            if (amigo == null || amigo.Id == miId) return false;

            // - Si ya somos amigos de antes
            bool yaSonAmigos = await _repository.SonAmigosAsync(miId, amigo.Id);
            if (yaSonAmigos) return false;

            // 2. Si pasa las validaciones, creamos la relación
            var nuevaAmistad = new Amistad
            {
                SolicitanteId = miId,
                ReceptorId = amigo.Id,
                FechaAmistad = DateTime.UtcNow,
                Aceptada = false
            };

            await _repository.AgregarAmistadAsync(nuevaAmistad);
            return true;
        }

        public async Task<List<UsuarioPublicoDto>> ObtenerMisAmigosAsync(int userId)
        {
            var amigos = await _repository.GetAmigosDeUsuarioAsync(userId);

            // Mapeamos la lista de Usuarios a UsuariosDto
            return amigos.Select(u => new UsuarioPublicoDto
            {
                Nombre = u.Nombre,
                AvatarId = u.AvatarId,
                FechaRegistro = u.FechaRegistro,
                PuntosTotales = u.PuntosTotales,
                RachaDias = u.RachaDias,
                RangoGeneral = u.RangoGeneral.ToString(),
                EsPro = u.EsPro(),
                Biografia = u.Biografia,
                CodigoAmigo = u.CodigoAmigo
            }).ToList();
        }

        public async Task<List<UsuarioPublicoDto>> ObtenerSolicitudesPendientesAsync(int userId)
        {
            var solicitantes = await _repository.GetSolicitudesPendientesAsync(userId);

            // Convertimos a DTO para que se vea bonito (nombre, avatar...)
            return solicitantes.Select(u => new UsuarioPublicoDto
            {
                Nombre = u.Nombre,
                AvatarId = u.AvatarId,
                FechaRegistro = u.FechaRegistro,
                PuntosTotales = u.PuntosTotales,
                RachaDias = u.RachaDias,
                RangoGeneral = u.RangoGeneral.ToString(),
                EsPro = u.EsPro(),
                Biografia = u.Biografia,
                CodigoAmigo = u.CodigoAmigo
            }).ToList();
        }

        public async Task<bool> ResponderSolicitudAsync(int miId, string codigoAmigoSolicitante, bool aceptar)
        {
            var solicitante = await _repository.GetByCodigoAmigoAsync(codigoAmigoSolicitante);
            if (solicitante == null) return false;

            // Buscamos la "hoja de papel" que une a los dos
            var amistad = await _repository.GetAmistadEntreUsuariosAsync(solicitante.Id, miId);
            if (amistad == null) return false;

            if (aceptar)
            {
                // CASO ACEPTAR: Cambiamos el estado a TRUE
                amistad.Aceptada = true;
                await _repository.UpdateAmistadAsync(amistad); 
            }
            else
            {
                // CASO RECHAZAR: Rompemos la hoja (Borrar fila)
                await _repository.EliminarAmistadAsync(amistad);
            }

            return true;
        }

        public async Task<bool> ActualizarPlanAsync(int userId, PlanSuscripcion nuevoPlan)
        {
            var usuario = await _repository.GetByIdAsync(userId);
            if (usuario == null) return false;

            usuario.PlanActual = nuevoPlan;

            if (nuevoPlan == PlanSuscripcion.ProMensual)
            {
                usuario.FechaFinSuscripcion = DateTime.Now.AddDays(30);
            }
            else
            {
                usuario.FechaFinSuscripcion = DateTime.MinValue;
            }

            await _repository.UpdateUsuarioAsync(usuario);
            return true;
        }

        
    }
}