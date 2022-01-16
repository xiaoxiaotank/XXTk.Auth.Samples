using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using XXTk.Auth.Samples.JwtBearer.HttpApi.Dtos;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto.UserName != dto.Password)
            {
                return Unauthorized();
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(JwtClaimTypes.Id, Guid.NewGuid().ToString("N")),
                    new Claim(JwtClaimTypes.Name, dto.UserName),
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenObj = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenObj);

            return Ok(new { token });
        }
    }
}
