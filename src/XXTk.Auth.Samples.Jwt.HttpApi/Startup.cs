using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using XXTk.Auth.Samples.JwtBearer.HttpApi.Authentication.JwtBearer;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi
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
            services.Configure<JwtOptions>(Configuration.GetSection(JwtOptions.Name));
            var jwtOptions = Configuration.GetSection(JwtOptions.Name).Get<JwtOptions>();

            services.AddSingleton(sp => new SigningCredentials(jwtOptions.SymmetricSecurityKey, jwtOptions.Algorithms));
            services.AddScoped<AppJwtBearerEvents>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // ��Ч��ǩ���ߣ�����Ҫ��֤ǩ����ʱ���Ὣ����token�е�ǩ���߽��жԱ�
                        ValidIssuer = jwtOptions.Issuer,
                        // ����ָ�������Ч��ǩ����
                        //ValidIssuers = new [] { jwtOptions.Issuer },
                        // �Ƿ���֤ǩ����
                        // Ĭ�� true
                        ValidateIssuer = true,

                        // ��Ч�����ڣ�����Ҫ��֤����ʱ���Ὣ����token�е����ڽ��жԱ�
                        ValidAudience = jwtOptions.Audience,
                        // ����ָ�������Ч������
                        //ValidAudiences = new[] { jwtOptions.Audience };
                        // �Ƿ���֤����
                        // ���ָ����ֵ��token�е� aud ������ƥ�䣬��token�����ܾ�
                        // Ĭ�� true
                        ValidateAudience = true,

                        // ǩ��������tokenǩ������Կ
                        IssuerSigningKey = jwtOptions.SymmetricSecurityKey,
                        // �Ƿ���֤ǩ��������tokenǩ������Կ
                        // Ĭ�� false
                        ValidateIssuerSigningKey = true,

                        // �Ƿ���֤token�Ƿ�����Ч����
                        // Ĭ�� true
                        ValidateLifetime = true,
                        
                        // ���� HttpContext.User.Identity.NameClaimType������ HttpContext.User.Identity.Name ȡ����ȷ��ֵ
                        NameClaimType = JwtClaimTypes.Name,
                        // ���� HttpContext.User.Identity.RoleClaimType������ HttpContext.User.Identity.IsInRole(xxx) ȡ����ȷ��ֵ
                        RoleClaimType = JwtClaimTypes.Role
                    };

                    // ��token��֤ͨ����ִ���� JwtBearerEvents.TokenValidated �󣩣�
                    // �Ƿ�token�洢�� Microsoft.AspNetCore.Authentication.AuthenticationProperties ��
                    options.SaveToken = true;

                    // ���ڣ�ָ��token�Ƿ������ĸ�Ⱥ��ģ�Ⱥ�巶Χ�������token���������Ȩ�޵���Դ����һ�飨��Դ��uri��
                    // ��������Ҫ���ڵ��䲻Ϊ�գ��� TokenValidationParameters.ValidAudience Ϊ��ʱ�����丳ֵ�� TokenValidationParameters.ValidAudience
                    // һ�㲻ʹ�ø�����
                    //options.Audience = jwtOptions.Audience;

                    //options.RefreshInterval = 

                    options.EventsType = typeof(AppJwtBearerEvents);
                });
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "XXTk.Auth.Samples.Jwt.HttpApi", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "XXTk.Auth.Samples.Jwt.HttpApi v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
