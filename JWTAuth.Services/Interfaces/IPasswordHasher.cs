using System.Linq.Expressions;

namespace JWTAuth.Core.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string storedHash, string password);
    }

}
