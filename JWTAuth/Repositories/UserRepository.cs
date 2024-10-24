using JWTAuth.Entities;
using Microsoft.AspNetCore.Identity;

public static class UserRepository
{
    private static PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public static User Get(string username, string password)
    {
        var users = new List<User>
        {
            new User { Id = 1, Username = "batman", Password = _passwordHasher.HashPassword(null, "batman") },
            new User { Id = 2, Username = "robin", Password = _passwordHasher.HashPassword(null, "robin") }
        };

        var user = users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user != null)
        {
            // Validar a senha com o PasswordHasher
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        return null;
    }
}
