using System.ComponentModel.DataAnnotations;

namespace REPS_backend.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        public string Nombre { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required, MinLength(6)]
        public string Password { get; set; }
        
    }
}