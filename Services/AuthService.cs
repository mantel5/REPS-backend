using Microsoft.IdentityModel.Tokens;
using REPS_backend.DTOs.Auth;
using REPS_backend.Models;
using REPS_backend.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace REPS_backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IConfiguration _config;

        public AuthService(IUsuarioRepository usuarioRepo, IConfiguration config)
        {
            _usuarioRepo = usuarioRepo;
            _config = config;
        }

        public async Task<string> RegistrarUsuarioAsync(RegisterDto dto)
        {
            if (await _usuarioRepo.ExistsByEmailAsync(dto.Email))
                throw new Exception("El email ya está registrado.");

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Rol = Rol.User, // Por defecto es usuario normal
                PlanActual = PlanSuscripcion.Gratuito,
                FechaFinSuscripcion = DateTime.UtcNow,
                PuntosTotales = 0,
                RachaDias = 0
            };

            nuevoUsuario.SetPassword(dto.Password);

            await _usuarioRepo.CrearUsuarioAsync(nuevoUsuario);

            return GenerarTokenJwt(nuevoUsuario);
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var usuario = await _usuarioRepo.GetByEmailAsync(dto.Email);

            if (usuario == null || !usuario.VerifyPassword(dto.Password))
            {
                return null; 
            }

            return GenerarTokenJwt(usuario);
        }

        private string GenerarTokenJwt(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}