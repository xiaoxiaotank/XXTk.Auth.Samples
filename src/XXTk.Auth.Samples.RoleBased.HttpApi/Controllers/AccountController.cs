using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.RoleBased.HttpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpPost("LoginForAdmin")]
        public async Task<IActionResult> LoginForAdmin()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "AdminOnly"),
                new Claim(ClaimTypes.Role, "Admin")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginForDeveloper")]
        public async Task<IActionResult> LoginForDeveloper()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "DeveloperOnly"),
                new Claim(ClaimTypes.Role, "Developer")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginForTester")]
        public async Task<IActionResult> LoginForTester()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "TesterOnly"),
                new Claim(ClaimTypes.Role, "Tester")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginForDeveloperAndTester")]
        public async Task<IActionResult> LoginForDeveloperAndTester()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, "DeveloperAndTester"),
                new Claim(ClaimTypes.Role, "Developer"),
                new Claim(ClaimTypes.Role, "Tester")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }
    }
}
