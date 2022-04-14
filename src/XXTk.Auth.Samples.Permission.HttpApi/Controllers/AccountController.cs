using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.Permission.HttpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpPost("LoginNonPermission")]
        public async Task<IActionResult> LoginNonPermission()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "NonPermission")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginCreatePermission")]
        public async Task<IActionResult> LoginCreatePermission()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "CreatePermission")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginUpdatePermission")]
        public async Task<IActionResult> LoginUpdatePermission()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "UpdatePermission")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginDeletePermission")]
        public async Task<IActionResult> LoginDeletePermission()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "DeletePermission")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginCreateAndUpdatePermission")]
        public async Task<IActionResult> LoginCreateAndUpdatePermission()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "CreateAndUpdatePermission")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("CreateAndDeletePermission")]
        public async Task<IActionResult> LoginCreateAndDeletePermission()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "CreateAndDeletePermission")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }

        [HttpPost("LoginAllPermission")]
        public async Task<IActionResult> LoginAllPermission()
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "AllPermission")
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok();
        }
    }
}
