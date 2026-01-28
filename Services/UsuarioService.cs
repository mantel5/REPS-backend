using REPS_backend.DTOs.Usuarios;
using REPS_backend.Repositories;

namespace REPS_backend.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        // TU PERFIL (Ve todo: email, código, etc.)
        public async Task<UsuarioPerfilDto?> ObtenerMiPerfilAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return null;

            return new UsuarioPerfilDto
            {
                Nombre = user.Nombre,
                Email = user.Email,
                AvatarId = user.AvatarId,
                CodigoAmigo = user.CodigoAmigo, 
                FechaRegistro = user.FechaRegistro,
                Rol = user.Rol,
                
                // Gamificación
                PuntosTotales = user.PuntosTotales,
                RachaDias = user.RachaDias,
                RangoGeneral = user.RangoGeneral,
                EsPro = user.EsPro()
            };
        }

        // PERFIL DE AMIGO (Solo ve lo bonito)
        public async Task<UsuarioPublicoDto?> BuscarAmigoPorCodigoAsync(string codigo)
        {
            var user = await _repository.GetByCodigoAmigoAsync(codigo.ToUpper()); 
            if (user == null) return null;

            return new UsuarioPublicoDto
            {
                Nombre = user.Nombre,
                AvatarId = user.AvatarId,
                FechaRegistro = user.FechaRegistro,
                
                // Gamificación
                PuntosTotales = user.PuntosTotales,
                RachaDias = user.RachaDias,
                RangoGeneral = user.RangoGeneral,
                EsPro = user.EsPro()
            };
        }

        public async Task<bool> ActualizarPerfilAsync(int id, UsuarioUpdateDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            // Solo actualizamos lo permitido
            if (!string.IsNullOrEmpty(dto.Nombre)) user.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.AvatarId)) user.AvatarId = dto.AvatarId;

            await _repository.UpdateUsuarioAsync(user);
            return true;
        }
    }
}