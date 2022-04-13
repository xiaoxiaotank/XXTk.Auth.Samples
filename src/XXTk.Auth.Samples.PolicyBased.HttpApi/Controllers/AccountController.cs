using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpPost("LoginForUnder18Age")]
        public async Task<IActionResult> LoginForUnder18Age()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "Under18Age"),
                new Claim(ClaimTypes.DateOfBirth, "2020-1-1")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginForOver18Age")]
        public async Task<IActionResult> LoginForOver18Age()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "Over18Age"),
                new Claim(ClaimTypes.DateOfBirth, "2000-1-1")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginForUnder18AgeAndInternetBarBoss")]
        public async Task<IActionResult> LoginForUnder18AgeAndInternetBarBoss()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "Under18AgeAndInternetBarBoss"),
                new Claim(ClaimTypes.DateOfBirth, "2020-1-1"),
                new Claim(ClaimTypes.Role, "InternetBarBoss")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpGet("NonAuthorize")]
        public string NonAuthorize()
        {
            return "NonAuthorize";
        }

        [HttpGet("DefaultAuthorize")]
        [Authorize]
        public string DefaultAuthorize()
        {
            return "DefaultAuthorize";
        }
    }
}
