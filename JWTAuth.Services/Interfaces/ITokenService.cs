using JWTAuth.Core.Services.Jwt;
using JWTAuth.Core.Services.Jwt.Models;
using static JWTAuth.Core.Services.Jwt.Manager.TokenService;
using JWTAuth.Entities;

namespace JWTAuth.Core.Interfaces
{
    public interface ITokenService
    {
        public CredentialToken GenerateToken(User userIdentity, TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations);
    }
}
