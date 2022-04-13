using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi.Authentication.JwtBearer;
using XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi.Dtos;

namespace XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthTokenService _authTokenService;

        public AccountController(IAuthTokenService authTokenService)
        {
            _authTokenService = authTokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
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

            var token = await _authTokenService.CreateAuthTokenAsync(user);

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] AuthTokenDto dto)
        {
            try
            {
                var token = await _authTokenService.RefreshAuthTokenAsync(dto);

                return Ok(token);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
