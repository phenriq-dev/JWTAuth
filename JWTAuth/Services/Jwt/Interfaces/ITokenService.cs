using JWTAuth.Models;
using JWTAuth.Services.Jwt.Models;
using static JWTAuth.Services.Jwt.Manager.TokenService;

namespace JWTAuth.Services.Jwt.Interfaces
{
    public interface ITokenService
    {
        public CredentialToken GenerateToken(User userIdentity, TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations);
    }
}
