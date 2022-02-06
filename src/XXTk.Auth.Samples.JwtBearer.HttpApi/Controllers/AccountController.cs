using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using XXTk.Auth.Samples.JwtBearer.HttpApi.Dtos;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtBearerOptions _jwtBearerOptions;
        private readonly JwtOptions _jwtOptions;
        private readonly SigningCredentials _signingCredentials;

        public AccountController(
            IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions,
            IOptionsSnapshot<JwtOptions> jwtOptions,
            SigningCredentials signingCredentials)
        {
            _jwtBearerOptions = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
            _jwtOptions = jwtOptions.Value;
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
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                // 必须 Utc，默认1小时
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes),
                SigningCredentials = _signingCredentials,
                //EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Total Bytes Length At Least 256!")), JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes128CbcHmacSha256)
            };

            var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                ?? new JwtSecurityTokenHandler();
            var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
            var token = handler.WriteToken(securityToken);

            return token;
        }

        /// <summary>
        /// 生成Rsa密钥对（不要把它放到正式环境中）
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("/rsa")]
        public IActionResult GenerateRsaKeyParies([FromServices] IWebHostEnvironment env)
        {
            RSAParameters privateKey, publicKey;

            // >= 2048 否则长度太短不安全
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    privateKey = rsa.ExportParameters(true);
                    publicKey = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            var dir = Path.Combine(env.ContentRootPath, "Rsa");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            System.IO.File.WriteAllText(Path.Combine(dir, "key.private.json"), JsonConvert.SerializeObject(privateKey));
            System.IO.File.WriteAllText(Path.Combine(dir, "key.public.json"), JsonConvert.SerializeObject(publicKey));

            return Ok();
        }
    }
}
