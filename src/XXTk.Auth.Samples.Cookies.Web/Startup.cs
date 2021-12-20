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
                    // 登录路径
                    options.LoginPath = "/Account/Login";
                    // 登出路径
                    options.LogoutPath = "/Account/Logout";
                    // 禁止访问路径
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    // returnUrl参数名，默认 returnUrl
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

                    // cookie 在浏览器中保存的时间
                    //options.Cookie.MaxAge = TimeSpan.FromMinutes(30);

                    // cookie 中 authentication ticket 的有效期
                    // 默认 14 天
                    options.ExpireTimeSpan = TimeSpan.FromSeconds(10);

                    // 是否滑动过期，默认 true
                    options.SlidingExpiration = true;

                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
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
