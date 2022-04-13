using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
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
            // �� redis.json �������޸�Ϊ�Լ�������
            //var redisConfig = Configuration.GetSection("Redis").Get<RedisConfiguration>();
            //services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(redisConfig);

            #region ͨ��Redis�洢���ݱ�����Կ�������Ⱥ�ͷֲ�ʽ���������ݽ�������

            //var redis = ConnectionMultiplexer.Connect(redisConfig.ConfigurationOptions);
            //services.AddDataProtection()
            //    .SetApplicationName(Assembly.GetEntryAssembly().FullName)
            //    .PersistKeysToStackExchangeRedis(redis, "dp-key");

            // ͨ��  lrange dp-key 0 -1 �鿴�������Կ 
            #endregion

            // ����Redis��ʵ�ֲַ�ʽ���棬���ڴ洢�Ự��Ϣ
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.ConfigurationOptions = redisConfig.ConfigurationOptions;
            //});

            // ��ѡ�������������Ϊ��������ʱʹ�� DI���� 
            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<IDataProtectionProvider, IDistributedCache>((options, dp, distributedCache) =>
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
                    // Ʊ���Ƿ������ͨ����Expires�뵱ǰʱ�����Ƚ����жϵ�
                    // ������ AuthenticationProperties.Persistent = true ʱ��
                    //      ͨ��(AuthenticationProperties.IssuedUtc + ExpireTimeSpan)�õ�һ�������ʱ��㣬����Ϊ Cookie �� Expires ���ԣ�
                    //      ��ʱ�����û������ MaxAge�����ֵҲ��Cookie����Ч��
                    // Ĭ�� 14 ��
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);

                    // Cooke ������ Expires��ָʾ Cookie ��������еı���ʱ��
                    // Ŀǰ���ֶ��ѱ����ã�Ӧ��ʹ�� ExpireTimeSpan��ֻ�е�Ʊ�ݳ־û�ʱ�������õ� Expires ������
                    //options.Cookie.Expiration = TimeSpan.FromMinutes(30);

                    // Cookie ��������еı���ʱ��
                    // ��� MaxAge �� Expires ͬʱ���ã����� MaxAge Ϊ׼
                    // ����������߾�δ���ã���� Cookie ��������Ϊ session cookie����������رպ�Cookie�ᱻ��գ�����������лỰ�ָ����ܣ�
                    // һ��������ʽ���ø��ֶ�
                    //options.Cookie.MaxAge = TimeSpan.FromDays(14);

                    // Cookie �Ĺ��ڷ�ʽ�Ƿ�Ϊ��������
                    // ����Ϊ true ʱ��������յ����������� Cookie ���������Ѿ�������һ�룬����˻����°䷢ Cookie��ע��authentication ticket Ҳ���µģ�
                    // Ĭ�� true
                    options.SlidingExpiration = true;

                    // Cookie �����֣�Ĭ���� .AspNetCore.Cookies
                    options.Cookie.Name = "auth";

                    // ��Ȩ�޷��� Cookie ����
                    //options.Cookie.Domain = ".xxx.cn";

                    // ��Ȩ�޷��� Cookie ��·��
                    options.Cookie.Path = "/";

                    // ���� ��վ�ĵ�����վ�� �� ��ǰվ�� ����GET����ʱ Я��Cookie ����ֹCSRF������
                    // Ĭ�� SameSiteMode.Lax
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;

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

                    // ��¼ǰ�ص�
                    options.Events.OnSigningIn = context =>
                    {
                        Console.WriteLine($"{context.Principal.Identity.Name} ���ڵ�¼...");
                        return Task.CompletedTask;
                    };

                    // ��¼��ص�
                    options.Events.OnSignedIn = context =>
                    {
                        Console.WriteLine($"{context.Principal.Identity.Name} �ѵ�¼");
                        return Task.CompletedTask;
                    };

                    // ע��ʱ�ص�
                    options.Events.OnSigningOut = context =>
                    {
                        Console.WriteLine($"{context.HttpContext.User.Identity.Name} ע��");
                        return Task.CompletedTask;
                    };

                    // ��֤ Principal ʱ�ص�
                    options.Events.OnValidatePrincipal = async context =>
                    {
                        Console.WriteLine($"{context.Principal.Identity.Name} ��֤ Principal");

                        var userId = context.Principal.Identities.First().Claims.First(c => c.Type == JwtClaimTypes.Id).Value;
                        var cacheKey = $"delete-auth-cookie:{userId}";
                        if (!string.IsNullOrWhiteSpace(await distributedCache.GetStringAsync(cacheKey)))
                        {
                            context.RejectPrincipal();
                            await distributedCache.RemoveAsync(cacheKey);
                        }
                    };

                    options.Events.OnRedirectToLogin = async context =>
                    {
                        if (IsAjaxRequest(context.Request))
                        {
                            context.Response.Headers[HeaderNames.Location] = context.RedirectUri;
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsJsonAsync(new { Message = "��¼�ѹ��ڣ������µ�¼" });
                        }
                        else
                        {
                            // ��Ӷ������
                            //context.RedirectUri += $"&now={DateTime.Now:o}";

                            context.Response.Redirect(context.RedirectUri);
                        }
                    };

                    // ��ȡ��ע�ͣ����Ự��Ϣ�洢�ڷ�����ڴ�
                    //options.SessionStore = new MemoryCacheTicketStore(options.ExpireTimeSpan);
                    // ��ȡ��ע�ͣ����Ự��Ϣ�洢�ڷ���˷ֲ�ʽ����
                    //options.SessionStore = new DistributedCacheTicketStore(distributedCache, options.ExpireTimeSpan);
                });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            // Cookieȫ�ֲ���
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

            services.AddAuthorization(options =>
            {
                // ��Ȩ���ã����¾�ʹ�õ�Ĭ��ֵ

                // Ĭ�ϵ���Ȩ����
                // Ĭ��ΪҪ��ͨ�������֤���û�
                options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                // �����ڶ����Ȩ������ʱ��������һ��ʧ�ܺ󣬺����Ĵ������Ƿ񻹼���ִ��
                // Ĭ�� true
                options.InvokeHandlersAfterFailure = true;
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

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return string.Equals(request.Query[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.Ordinal) ||
                string.Equals(request.Headers[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.Ordinal);
        }
    }
}
