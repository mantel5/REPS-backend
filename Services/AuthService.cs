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
                CodigoAmigo = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
                FechaRegistro = DateTime.UtcNow,
                Rol = Rol.User, 
                PlanActual = PlanSuscripcion.Gratuito,
                FechaFinSuscripcion = DateTime.UtcNow,
                PuntosTotales = 0,
                RachaDias = 0
            };

            nuevoUsuario.SetPassword(dto.Password);
            await _usuarioRepo.CrearUsuarioAsync(nuevoUsuario);

            return GenerateToken(nuevoUsuario);
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var usuario = await _usuarioRepo.GetByEmailAsync(dto.Email);

            if (usuario == null || !usuario.VerifyPassword(dto.Password))
            {
                return null;
            }

            return GenerateToken(usuario);
        }

        public bool HasAccessToResource(int requestedUserId, ClaimsPrincipal user)
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return false; 
            }

            var isOwnResource = userId == requestedUserId;

            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var isAdmin = roleClaim != null && roleClaim.Value == Rol.Admin; // Usamos tu constante de Rol

            return isOwnResource || isAdmin;
        }

        private string GenerateToken(Usuario usuario)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol) 
                }),
                
                Expires = DateTime.UtcNow.AddDays(7),
                
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }
    }
}