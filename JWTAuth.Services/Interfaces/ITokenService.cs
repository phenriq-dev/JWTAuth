using JWTAuth.Core.Services.Jwt;
using JWTAuth.Core.Services.Jwt.Models;
using JWTAuth.Entities;
using static JWTAuth.Core.Services.Jwt.Manager.TokenService;

namespace JWTAuth.Core.Interfaces
{
    public interface ITokenService
    {
        public CredentialToken GenerateToken(User userIdentity, TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations);
    }
}
