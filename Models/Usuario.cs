
public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public int PuntosTotales { get; set; } 
    public string Rol { get; set; } = "User";
     

    // Método para asignar la contraseña de forma segura
    public void SetPassword(string password)
    {
        // Genera el hash y lo asigna a PasswordHash
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Método para verificar la contraseña
    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }
}
