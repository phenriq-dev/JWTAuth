using JWTAuth.Core.Interfaces;
using JWTAuth.Core.Services.Jwt.Models;
using JWTAuth.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

namespace JWTAuth.Core.Services.Jwt.Manager
{
    public class TokenService : ITokenService
    {
        public CredentialToken GenerateToken(User userIdentity, TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations, IDistributedCache _cache, string refreshToken = null)
        {
            var jti = Guid.NewGuid().ToString("N");

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(userIdentity.Username, "Login"),
                new[]
                {
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Sub, userIdentity.Username),
                new Claim("UserId", userIdentity.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                }
            );

            DateTime created = DateTime.UtcNow;
            DateTime expiration = created.AddSeconds(tokenConfigurations.Seconds);

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

            var accessToken = handler.WriteToken(securityToken);
            var newRefreshToken = Guid.NewGuid().ToString("N") + userIdentity.UserId;

            CredentialToken result = new CredentialToken
            {
                authenticated = true,
                created = created.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = expiration.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = accessToken,
                refreshToken = newRefreshToken,
                message = "OK"
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenConfigurations.Seconds),
                SlidingExpiration = TimeSpan.FromSeconds(tokenConfigurations.Seconds)
            };

            var refreshTokenData = new RefreshTokenData
            {
                RefreshToken = newRefreshToken,
                ID = userIdentity.UserId
            };

            if (!string.IsNullOrEmpty(refreshToken))
            {
                _cache.Remove($"refreshToken:{userIdentity.UserId}");
            }

            _cache.SetString($"refreshToken:{userIdentity.UserId}", JsonSerializer.Serialize(refreshTokenData), cacheOptions);

            return result;
        }

        public async Task<CredentialToken> RefreshTokenAsync(
            User userIdentity,
            TokenConfigurations tokenConfigurations,
            SigningConfigurations signingConfigurations,
            IDistributedCache _cache,
            string refreshToken)
        {
            var cachedData = await _cache.GetStringAsync("refreshToken:" + refreshToken);

            if (string.IsNullOrEmpty(cachedData))
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var refreshTokenData = JsonSerializer.Deserialize<RefreshTokenData>(cachedData);

            if (refreshTokenData == null || refreshTokenData.ID != userIdentity.UserId)
                throw new UnauthorizedAccessException("Invalid refresh token data.");

            var newToken = GenerateToken(userIdentity, tokenConfigurations, signingConfigurations, _cache);

            return newToken;
        }


    }
}
