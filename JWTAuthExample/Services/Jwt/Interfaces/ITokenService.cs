using JWTAuthExample.Models;
using JWTAuthExample.Services.Jwt.Models;
using static JWTAuthExample.Services.Jwt.Manager.TokenService;

namespace JWTAuthExample.Services.Jwt.Interfaces
{
    public interface ITokenService
    {
        public CredentialToken GenerateToken(User userIdentity, TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations);
    }
}
