using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.ClaimsBased.HttpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpPost("LoginForRankClaimP3")]
        public async Task<IActionResult> LoginForRankClaimP3()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "RankClaimP3"),
                new Claim("Rank", "P3")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginForRankClaimM3")]
        public async Task<IActionResult> LoginForRankClaimM3()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "RankClaimM3"),
                new Claim("Rank", "M3")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginForRankClaimP3AndM3")]
        public async Task<IActionResult> LoginForRankClaimP3AndM3()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "RankClaimP3AndM3"),
                new Claim("Rank", "P3"),
                new Claim("Rank", "M3")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }
    }
}
