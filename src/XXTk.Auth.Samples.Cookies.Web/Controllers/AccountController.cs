using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using XXTk.Auth.Samples.Cookies.Web.Models;

namespace XXTk.Auth.Samples.Cookies.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOptionsMonitor<CookieAuthenticationOptions> _cookieAuthOptionsMonitor;

        public AccountController(IOptionsMonitor<CookieAuthenticationOptions> cookieAuthOptions)
        {
            _cookieAuthOptionsMonitor = cookieAuthOptions;
        } 

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

            // 登录
            // 内部会自动对 cookie 进行加密
            var properties = new AuthenticationProperties
            {
                // 是否持久化。默认非持久化，即该Cookie有效期是会话级别
                // 注：只有设置为 true，下面的 ExpiresUtc 或全局配置的 options.ExpireTimeSpan 才会生效
                IsPersistent = input.RememberMe,

                // Cookie 中 authentication ticket 的过期时间
                // 重写 CookieAuthenticationOptions.ExpireTimeSpan 的值
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(60),
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

            #region 以下简要模拟 SignInAsync 内部细节，更多细节请查看 AuthenticationService 和 CookieAuthenticationHandler

            //var options = _cookieAuthOptionsMonitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);
            //var ticket = new AuthenticationTicket(principal, properties, CookieAuthenticationDefaults.AuthenticationScheme);
            //var cookieValue = options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding(HttpContext));

            //options.CookieManager.AppendResponseCookie(HttpContext, options.Cookie.Name, cookieValue, new CookieOptions());

            #endregion

            #region 添加自定义Cookie

            Response.Cookies.Append("author", "xiaoxiaotank", new CookieOptions
            {
                MaxAge = TimeSpan.FromSeconds(30)
            });
            #endregion

            if (Url.IsLocalUrl(input.ReturnUrl))
            {
                return Redirect(input.ReturnUrl);
            }

            return Redirect("/");
        }

        private static string GetTlsTokenBinding(HttpContext context)
        {
            var binding = context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
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
