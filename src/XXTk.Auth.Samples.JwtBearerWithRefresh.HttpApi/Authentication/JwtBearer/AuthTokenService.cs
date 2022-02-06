using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi.Dtos;

namespace XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi.Authentication.JwtBearer
{
    public class AuthTokenService : IAuthTokenService
    {
        private const string RefreshTokenIdClaimType = "refresh_token_id";

        private readonly JwtBearerOptions _jwtBearerOptions;
        private readonly JwtOptions _jwtOptions;
        private readonly SigningCredentials _signingCredentials;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<AuthTokenService> _logger;

        public AuthTokenService(
           IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions,
           IOptionsSnapshot<JwtOptions> jwtOptions,
           SigningCredentials signingCredentials,
           IDistributedCache distributedCache,
           ILogger<AuthTokenService> logger)
        {
            _jwtBearerOptions = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
            _jwtOptions = jwtOptions.Value;
            _signingCredentials = signingCredentials;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<AuthTokenDto> CreateAuthTokenAsync(UserDto user)
        {
            var result = new AuthTokenDto();
            var (refreshTokenId, refreshToken) = await CreateRefreshTokenAsync(user.Id);
            result.RefreshToken = refreshToken;
            result.AccessToken = CreateJwtToken(user, refreshTokenId);

            return result;
        }

        private string CreateJwtToken(UserDto user, string refreshTokenId)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(JwtClaimTypes.Id, user.Id),
                    new Claim(JwtClaimTypes.Name, user.UserName),
                    new Claim(RefreshTokenIdClaimType, refreshTokenId)
                }),
                Issuer = _jwtBearerOptions.TokenValidationParameters.ValidIssuer,
                Audience = _jwtBearerOptions.TokenValidationParameters.ValidAudience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiresMinutes),
                SigningCredentials = _signingCredentials,
            };

            var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                ?? new JwtSecurityTokenHandler();
            var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
            var token = handler.WriteToken(securityToken);

            return token;
        }

        private async Task<(string refreshTokenId, string refreshToken)> CreateRefreshTokenAsync(string userId)
        {
            var rnBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(rnBytes);

            var token = Convert.ToBase64String(rnBytes);

            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays));

            var tokenId = Guid.NewGuid().ToString("N");
            await _distributedCache.SetStringAsync(GetRefreshTokenKey(userId, tokenId), token, options);

            return (tokenId, token);

        }

        public async Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token)
        {
            var validationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false;

            var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                ?? new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            try
            {
                principal = handler.ValidateToken(token.AccessToken, validationParameters, out _);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                throw new BadHttpRequestException("Invalid access token");
            }

            var identity = principal.Identities.First();
            var userId = identity.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id).Value;
            var refreshTokenId = identity.Claims.FirstOrDefault(c => c.Type == RefreshTokenIdClaimType).Value;
            var refreshTokenKey = GetRefreshTokenKey(userId, refreshTokenId);
            var refreshToken = await _distributedCache.GetStringAsync(refreshTokenKey);
            if (refreshToken != token.RefreshToken)
            {
                throw new BadHttpRequestException("Invalid refresh token");
            }

            await _distributedCache.RemoveAsync(refreshTokenKey);

            // 这里应该是从数据库中根据 userId 获取用户信息
            var user = new UserDto()
            {
                Id = userId,
                UserName = principal.Identity.Name
            };

            return await CreateAuthTokenAsync(user);
        }

        private string GetRefreshTokenKey(string userId, string refreshTokenId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

            return $"{userId}:{refreshTokenId}";
        }
    }
}
