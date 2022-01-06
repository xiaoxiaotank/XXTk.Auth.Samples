using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.Cookies.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    // 以下是全局配置，部分项可以在实际使用时重写

                    // 以下4个配置均在 CookieAuthenticationDefaults 类中有对应的默认值
                    // 登录路径
                    options.LoginPath = new PathString("/Account/Login");
                    // 注销路径
                    options.LogoutPath = new PathString("/Account/Logout");
                    // 禁止访问路径
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    // returnUrl参数名，默认 ReturnUrl
                    options.ReturnUrlParameter = "returnUrl";

                    // cookie 在浏览器中保存的时间
                    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);

                    // cookie 中 authentication ticket 的有效期
                    // 默认 14 天
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);

                    // 是否滑动过期，默认 true
                    options.SlidingExpiration = true;

                    // 允许 跨站的第三方站点 向 当前站点 发送GET请求时 携带Cookie （防止CSRF攻击）
                    // 默认 SameSiteMode.Lax
                    options.Cookie.SameSite = SameSiteMode.Lax;

                    // 禁止客户端脚本访问Cookie（防止XSS攻击）
                    // 默认 true
                    options.Cookie.HttpOnly = true;

                    // 只有当颁发 Cookie 的 URI 是 Https 时，才设置 Secure = true
                    // 默认 CookieSecurePolicy.SameAsRequest
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                    // 指示 Cookie 对于应用的正常运行是必要的
                    // 默认 true
                    options.Cookie.IsEssential = true;
                });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
