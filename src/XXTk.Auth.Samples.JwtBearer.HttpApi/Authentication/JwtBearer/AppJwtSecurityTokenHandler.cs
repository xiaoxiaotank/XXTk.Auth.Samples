using IdentityModel;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi.Authentication.JwtBearer
{
    public class AppJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        private readonly IDistributedCache _distributedCache;

        public AppJwtSecurityTokenHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var principal = base.ValidateToken(token, validationParameters, out validatedToken);

            var jsonWebToken = validatedToken as JwtSecurityToken;

            var userId = ((ClaimsIdentity)principal.Identity).Claims.First(c => c.Type == JwtClaimTypes.Id).Value;
            ValidateIssuedAt(userId, jsonWebToken.IssuedAt);

            return principal;
        }

        protected virtual void ValidateIssuedAt(string userId, DateTime issuedAt)
        {
            var userString = _distributedCache.GetString(userId);
            if (!string.IsNullOrWhiteSpace(userString))
            {
                var user = JsonConvert.DeserializeObject<AuthUser>(_distributedCache.GetString(userId));
                if (user.LastModifyPasswordTime is not null)
                {
                    if (issuedAt < user.LastModifyPasswordTime)
                    {
                        throw LogHelper.LogExceptionMessage(new SecurityTokenValidationException("Password modified, please login again"));
                    }
                }
            }
        }

    }
}
