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
                    // ������ȫ�����ã������������ʵ��ʹ��ʱ��д

                    // ����4�����þ��� CookieAuthenticationDefaults �����ж�Ӧ��Ĭ��ֵ
                    // ��¼·��
                    options.LoginPath = new PathString("/Account/Login");
                    // ע��·��
                    options.LogoutPath = new PathString("/Account/Logout");
                    // ��ֹ����·��
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    // returnUrl��������Ĭ�� ReturnUrl
                    options.ReturnUrlParameter = "returnUrl";

                    // cookie ��������б����ʱ��
                    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);

                    // cookie �� authentication ticket ����Ч��
                    // Ĭ�� 14 ��
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);

                    // �Ƿ񻬶����ڣ�Ĭ�� true
                    options.SlidingExpiration = true;

                    // ���� ��վ�ĵ�����վ�� �� ��ǰվ�� ����GET����ʱ Я��Cookie ����ֹCSRF������
                    // Ĭ�� SameSiteMode.Lax
                    options.Cookie.SameSite = SameSiteMode.Lax;

                    // ��ֹ�ͻ��˽ű�����Cookie����ֹXSS������
                    // Ĭ�� true
                    options.Cookie.HttpOnly = true;

                    // ֻ�е��䷢ Cookie �� URI �� Https ʱ�������� Secure = true
                    // Ĭ�� CookieSecurePolicy.SameAsRequest
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                    // ָʾ Cookie ����Ӧ�õ����������Ǳ�Ҫ��
                    // Ĭ�� true
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
