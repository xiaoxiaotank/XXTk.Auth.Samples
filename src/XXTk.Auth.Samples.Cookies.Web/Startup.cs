using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
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
            // ��ѡ�������������Ϊ��������ʱʹ�� DI���� 
            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<IDataProtectionProvider>((options, dp) =>
                {
                    // set-cookie: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie
                    // Ĭ�����òο�ASP.NET CoreԴ�룺class CookieAuthenticationOptions
                    // Ĭ�Ϻ������òο�ASP.NET CoreԴ�룺class PostConfigureCookieAuthenticationOptions

                    // �����ǵ�ǰ Cookie ��֤������ȫ�����ã������������ʵ��ʹ��ʱ��д

                    // ����4�����þ��� CookieAuthenticationDefaults �����ж�Ӧ��Ĭ��ֵ
                    // ��¼·��
                    options.LoginPath = new PathString("/Account/Login");
                    // ע��·��
                    options.LogoutPath = new PathString("/Account/Logout");
                    // ��ֹ����·��
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    // returnUrl��������Ĭ�� ReturnUrl
                    options.ReturnUrlParameter = "returnUrl";

                    // Cookie �� authentication ticket ����Ч�ڣ�ע������Cookie����Ч�ڡ���Ȼ����� Cookie ��û�ˣ���Ҳ��ʧЧ�ˣ�
                    // ��δ������֤�ỰΪ �־û���Persistent��������ֶ���Ч����ʱ ticket ����Ч���� Cookie ����Ч�ڱ���һ��
                    // ��� MaxAge �� Expires ��δ���ã���������֤�ỰΪ �־û����򽫸�ֵ����Ϊ MaxAge
                    // Ĭ�� 14 ��
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);

                    // Expires��Ŀǰ���ֶ��ѱ�����
                    //options.Cookie.Expiration = TimeSpan.FromMinutes(30);

                    // Cookie ��������еı���ʱ��
                    // ��� MaxAge �� Expires ͬʱ���ã����� MaxAge Ϊ׼
                    // ����������߾�δ���ã���� Cookie ��������Ϊ session cookie����������رպ�Cookie�ᱻ��գ�����������лỰ�ָ����ܣ�
                    // һ��������ʽ���ø��ֶ�
                    //options.Cookie.MaxAge = TimeSpan.FromDays(14);

                    // Cookie �Ĺ��ڷ�ʽ�Ƿ�Ϊ��������
                    // ����Ϊ true ʱ��������յ����������� Cookie ���������Ѿ�������һ�룬����˻����°䷢ Cookie��ע��authentication ticket Ҳ���µģ�
                    // Ĭ�� true
                    //options.SlidingExpiration = true;

                    // Cookie �����֣�Ĭ���� .AspNetCore.Cookies
                    options.Cookie.Name = "auth";

                    // ��Ȩ�޷��� Cookie ����
                    //options.Cookie.Domain = ".xxx.cn";

                    // ��Ȩ�޷��� Cookie ��·��
                    //options.Cookie.Path = "/Home";

                    // ���� ��վ�ĵ�����վ�� �� ��ǰվ�� ����GET����ʱ Я��Cookie ����ֹCSRF������
                    // Ĭ�� SameSiteMode.Lax
                    options.Cookie.SameSite = SameSiteMode.Lax;

                    // ��ֹ�ͻ��˽ű�����Cookie����ֹXSS������
                    // Ĭ�� true
                    options.Cookie.HttpOnly = true;

                    // ֻ�е��䷢ Cookie �� URI �� Https ʱ�������� Secure = true
                    // Ĭ�� CookieSecurePolicy.SameAsRequest
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                    // ָʾ�� Cookie ����Ӧ�õ����������Ǳ�Ҫ�ģ�����Ҫ�����û�ͬ��ʹ��
                    // Ĭ�� true
                    options.Cookie.IsEssential = true;

                    // ���� Cookie ������
                    // Ĭ�� ChunkingCookieManager
                    options.CookieManager = new ChunkingCookieManager();

                    // ���ڼ��ܺͽ��� Cookie �е� authentication ticket
                    // ����ΪĬ��ֵ
                    options.DataProtectionProvider ??= dp;
                    var dataProtector = options.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", CookieAuthenticationDefaults.AuthenticationScheme, "v2");
                    options.TicketDataFormat = new TicketDataFormat(dataProtector);

                    // ��¼ʱ�ص�
                    options.Events.OnSigningIn = context =>
                    {
                        Console.WriteLine($"{context.Principal.Identity.Name} ���ڵ�¼...");
                        return Task.CompletedTask;
                    };

                    options.Events.OnSignedIn = context =>
                    {
                        Console.WriteLine($"{context.Principal.Identity.Name} �ѵ�¼");
                        return Task.CompletedTask;
                    };
                });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddCookiePolicy(options =>
            {
                options.OnAppendCookie = context =>
                {
                    Console.WriteLine("------------------ On Append Cookie --------------------");
                    Console.WriteLine($"Name: {context.CookieName}\tValue: {context.CookieValue}");
                };
                options.OnDeleteCookie = context =>
                {
                    Console.WriteLine("------------------ On Delete Cookie --------------------");
                    Console.WriteLine($"Name: {context.CookieName}");
                };
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

            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("Cookies:.........................");

            //    var cookies = context.Request.Cookies;
            //    foreach (var cookie in cookies)
            //    {
            //        Console.WriteLine($"Name:{cookie.Key}\tValue:{cookie.Value}");
            //    }

            //    await next();
            //});

            app.UseRouting();

            // Cookie �����м��
            app.UseCookiePolicy();

            // �����֤�м��
            app.UseAuthentication();
            // ��Ȩ�м��
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
