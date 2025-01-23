using JWTAuth.Core.Interfaces;

namespace JWTAuth.Core.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            // Verifica se a senha fornecida corresponde ao hash armazenado
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}