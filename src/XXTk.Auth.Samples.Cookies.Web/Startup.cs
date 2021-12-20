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
                    // ��¼·��
                    options.LoginPath = "/Account/Login";
                    // �ǳ�·��
                    options.LogoutPath = "/Account/Logout";
                    // ��ֹ����·��
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    // returnUrl��������Ĭ�� returnUrl
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

                    // cookie ��������б����ʱ��
                    //options.Cookie.MaxAge = TimeSpan.FromMinutes(30);

                    // cookie �� authentication ticket ����Ч��
                    // Ĭ�� 14 ��
                    options.ExpireTimeSpan = TimeSpan.FromSeconds(10);

                    // �Ƿ񻬶����ڣ�Ĭ�� true
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
