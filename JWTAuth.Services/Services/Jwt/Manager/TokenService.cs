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
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(userIdentity.Username, "Login"),
                new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userIdentity.Username),
                    new Claim("UserId", userIdentity.UserId.ToString())
                }
            );

            int seconds = tokenConfigurations.Seconds;
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
                refreshToken = Guid.NewGuid().ToString().Replace("-", string.Empty) + userIdentity.UserId.ToString(),
                message = "OK"
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds),
                SlidingExpiration = TimeSpan.FromSeconds(seconds)
            };

            var refreshTokenData = new RefreshTokenData();
            refreshTokenData.RefreshToken = result.refreshToken;
            refreshTokenData.ID = userIdentity.UserId;

            if (!string.IsNullOrEmpty(refreshToken))
            {
                _cache.Remove(refreshToken);
            }

            _cache.SetString(result.refreshToken, JsonSerializer.Serialize(refreshTokenData), cacheOptions);

            return result;
        }

        public async Task<CredentialToken> RefreshTokenAsync(User userIdentity, TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations, IDistributedCache _cache, string refreshToken)
        {
            var cachedRefreshTokenData = await _cache.GetStringAsync(refreshToken);

            if (string.IsNullOrEmpty(cachedRefreshTokenData))
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var refreshTokenData = JsonSerializer.Deserialize<RefreshTokenData>(cachedRefreshTokenData);

            if (refreshTokenData == null)
                throw new UnauthorizedAccessException("Invalid refresh token data.");

            if (refreshTokenData.ID == 0)
                throw new UnauthorizedAccessException("Invalid user ID associated with refresh token.");

            if (userIdentity == null)
                throw new UnauthorizedAccessException("User not found.");

            var newToken = GenerateToken(userIdentity, tokenConfigurations, signingConfigurations, _cache);

            var newRefreshTokenData = new RefreshTokenData
            {
                RefreshToken = newToken.refreshToken,
                ID = userIdentity.UserId
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenConfigurations.Seconds),
                SlidingExpiration = TimeSpan.FromSeconds(tokenConfigurations.Seconds)
            };

            await _cache.SetStringAsync(newRefreshTokenData.RefreshToken, JsonSerializer.Serialize(newRefreshTokenData), cacheOptions);

            newToken.refreshToken = newRefreshTokenData.RefreshToken;
            return newToken;
        }


    }
}
