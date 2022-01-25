using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
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
                        // 有效的签名算法列表，仅列出的算法是有效的
                        // 默认 null，即所有算法均可
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                        // 有效的token类型
                        // 默认 null，即所有类型均可
                        ValidTypes = new[] { JwtConstants.HeaderType },
                        
                        // 有效的签发者，当需要验证签发者时，会将其与token中的签发者进行对比
                        ValidIssuer = jwtOptions.Issuer,
                        // 可以指定多个有效的签发者
                        //ValidIssuers = new [] { jwtOptions.Issuer },
                        // 是否验证签发者
                        // 默认 true
                        ValidateIssuer = true,

                        // 有效的受众，当需要验证受众时，会将其与token中的受众进行对比
                        ValidAudience = jwtOptions.Audience,
                        // 可以指定多个有效的受众
                        //ValidAudiences = new[] { jwtOptions.Audience };
                        // 是否验证受众
                        // 如果指定的值与token中的 aud 参数不匹配，则token将被拒绝
                        // 默认 true
                        ValidateAudience = true,

                        // 签发者用于token签名的密钥
                        IssuerSigningKey = jwtOptions.SymmetricSecurityKey,
                        // 是否验证签发者用于token签名的密钥
                        // 默认 false
                        ValidateIssuerSigningKey = true,

                        // 是否要求token必须包含过期时间
                        // 默认 true
                        RequireExpirationTime = true,
                        // 是否验证token是否在有效期内（包括nbf和exp）
                        // 默认 true
                        ValidateLifetime = true,

                        // 设置 HttpContext.User.Identity.NameClaimType，便于 HttpContext.User.Identity.Name 取到正确的值
                        NameClaimType = JwtClaimTypes.Name,
                        // 设置 HttpContext.User.Identity.RoleClaimType，便于 HttpContext.User.Identity.IsInRole(xxx) 取到正确的值
                        RoleClaimType = JwtClaimTypes.Role,

                        // 时钟漂移
                        // 可以在验证token有效期时，允许一定的时间误差（如时间刚达到token中exp，但是允许未来5分钟内该token仍然有效）
                        // 默认 300s，即 5min
                        ClockSkew = TimeSpan.Zero,
                    };

                    // 当token验证通过后（执行完 JwtBearerEvents.TokenValidated 后），
                    // 是否将token存储在 Microsoft.AspNetCore.Authentication.AuthenticationProperties 中
                    // 默认 true
                    options.SaveToken = true;

                    // token验证处理器列表
                    // 默认 有1个 JwtSecurityTokenHandler
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());

                    // 受众，指该token是服务于哪个群体的（群体范围名），或该token所授予的有权限的资源是哪一块（资源的uri）
                    // 该属性主要用于当其不为空，但 TokenValidationParameters.ValidAudience 为空时，将其赋值给 TokenValidationParameters.ValidAudience
                    // 一般不使用该属性
                    //options.Audience = jwtOptions.Audience;    

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
