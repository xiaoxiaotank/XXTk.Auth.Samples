using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using XXTk.Auth.Samples.JwtBearer.HttpApi.Dtos;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtBearerOptions _jwtBearerOptions;
        private readonly SigningCredentials _signingCredentials;

        public AccountController(
            IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions,
            SigningCredentials signingCredentials)
        {
            _jwtBearerOptions = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
            _signingCredentials = signingCredentials;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto.UserName != dto.Password)
            {
                return Unauthorized();
            }

            var user = new UserDto()
            {
                Id = Guid.NewGuid().ToString("N"),
                UserName = dto.UserName
            };

            var token = CreateJwtToken(user);

            return Ok(new { token });
        }

        [NonAction]
        private string CreateJwtToken(UserDto user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(JwtClaimTypes.Id, user.Id),
                    new Claim(JwtClaimTypes.Name, user.UserName)
                }),
                Issuer = _jwtBearerOptions.TokenValidationParameters.ValidIssuer,
                Audience = _jwtBearerOptions.TokenValidationParameters.ValidAudience,
                Expires = DateTime.UtcNow.AddSeconds(10),
                SigningCredentials = _signingCredentials
            };

            var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault() 
                ?? new JwtSecurityTokenHandler();
            var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
            var token = handler.WriteToken(securityToken);

            return token;
        }
    }
}
