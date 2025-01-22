namespace JWTAuth.Core.Services.Jwt.Models
{
    public class LoginCredentials
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
