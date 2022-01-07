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
                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie

                    // 以下是当前 Cookie 方案全局配置，部分项可以在实际使用时重写

                    // 以下4个配置均在 CookieAuthenticationDefaults 类中有对应的默认值
                    // 登录路径
                    options.LoginPath = new PathString("/Account/Login");
                    // 注销路径
                    options.LogoutPath = new PathString("/Account/Logout");
                    // 禁止访问路径
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    // returnUrl参数名，默认 ReturnUrl
                    options.ReturnUrlParameter = "returnUrl";

                    // Cookie 中 authentication ticket 的有效期（注：不是Cookie的有效期，当然，如果 Cookie 都没了，它也就失效了）
                    // 仅当声明为 持久化（Persistent）时，该字段才会生效
                    // 如果 MaxAge 和 Expires 均未设置，则将该值设置为 MaxAge
                    // 默认 14 天
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);

                    // Expires，目前已无法使用
                    //options.Cookie.Expiration = TimeSpan.FromMinutes(30);

                    // Cookie 在浏览器中的保存时间
                    // 如果 MaxAge 和 Expires 同时设置，则以 MaxAge 为准
                    // 如果以上两者均未设置，则该 Cookie 生命周期为 session cookie，即浏览器关闭后，Cookie会被清空（部分浏览器有会话恢复功能）
                    // 一般无需显式设置该字段
                    //options.Cookie.MaxAge = TimeSpan.FromDays(14);

                    // Cookie 的过期方式是否为滑动过期
                    // 设置为 true 时，服务端收到请求，若发现 Cookie 的生存期已经超过了一半，服务端会重新颁发 Cookie（注：authentication ticket 也是新的）
                    // 默认 true
                    options.SlidingExpiration = true;

                    // Cookie 的名字，默认是 .AspNetCore.Cookies
                    options.Cookie.Name = "auth";

                    // 有权限访问 Cookie 的域
                    //options.Cookie.Domain = ".xxx.cn";

                    // 有权限访问 Cookie 的路径
                    //options.Cookie.Path = "/Home";

                    // 允许 跨站的第三方站点 向 当前站点 发送GET请求时 携带Cookie （防止CSRF攻击）
                    // 默认 SameSiteMode.Lax
                    options.Cookie.SameSite = SameSiteMode.Lax;

                    // 禁止客户端脚本访问Cookie（防止XSS攻击）
                    // 默认 true
                    options.Cookie.HttpOnly = true;

                    // 只有当颁发 Cookie 的 URI 是 Https 时，才设置 Secure = true
                    // 默认 CookieSecurePolicy.SameAsRequest
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                    // 指示该 Cookie 对于应用的正常运行是必要的，不需要经过用户同意使用
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
