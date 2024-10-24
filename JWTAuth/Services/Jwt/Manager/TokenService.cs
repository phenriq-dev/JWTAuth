using JWTAuth.Models;
using JWTAuth.Services.Jwt.Interfaces;
using JWTAuth.Services.Jwt.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace JWTAuth.Services.Jwt.Manager
{
    public class TokenService : ITokenService
    {
        public class CredentialToken
        {
            public bool authenticated { get; set; }
            public string created { get; set; }
            public string expiration { get; set; }
            public string accessToken { get; set; }
            public string refreshToken { get; set; }
            public string message { get; set; }
        }

        public CredentialToken GenerateToken(User userIdentity, TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(userIdentity.Username, "Login"),
                new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userIdentity.Username)
                }
            );

            int seconds = 1800;
            DateTime created = DateTime.Now;
            DateTime expiration = created + TimeSpan.FromSeconds(seconds);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenConfigurations.Issuer,
                Audience = tokenConfigurations.Audience,
                SigningCredentials = signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = created,
                Expires = expiration
            });

            var token = handler.WriteToken(securityToken);

            CredentialToken result = new CredentialToken
            {
                authenticated = true,
                created = created.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = expiration.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = token,
                refreshToken = Guid.NewGuid().ToString().Replace("-", String.Empty) + userIdentity.Id.ToString(),
                message = "OK"
            };

            return result;
        }
    }
}
