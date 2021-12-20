using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using XXTk.Auth.Samples.Cookies.Web.Models;

namespace XXTk.Auth.Samples.Cookies.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login([FromQuery] string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel input)
        {
            ViewBag.ReturnUrl = input.ReturnUrl;

            // 用户名密码相同视为登录成功
            if (input.UserName != input.Password)
            {
                ModelState.AddModelError("CustomError", "无效的用户名或密码");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, input.UserName)
            });

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                // 是否持久化。
                // 只有设置为 true，ExpiresUtc 才会生效
                IsPersistent = input.RememberMe,

                // authentication ticket 过期时间
                // 若未设置，则取 CookieAuthenticationOptions.ExpireTimeSpan
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(60),              
            });

            if (Url.IsLocalUrl(input.ReturnUrl))
            {
                return Redirect(input.ReturnUrl);
            }

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Redirect("/");
        }

        [HttpGet]
        public IActionResult AccessDenied([FromQuery] string returnUrl = null)
        {
            return View();
        }
    }
}
