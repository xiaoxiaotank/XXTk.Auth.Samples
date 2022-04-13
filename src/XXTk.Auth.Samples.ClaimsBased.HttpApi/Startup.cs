using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;

namespace XXTk.Auth.Samples.ClaimsBased.HttpApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "auth";
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                });

            services.AddAuthorization(options =>
            {
                // 仅要求具有声明 Rank
                options.AddPolicy("RankClaim", policy => policy.RequireClaim("Rank"));
                // 要求具有声明 Rank，且值为 P3
                options.AddPolicy("RankClaimP3", policy => policy.RequireClaim("Rank", "P3"));
                // 要求具有声明 Rank，且值为 M3
                options.AddPolicy("RankClaimM3", policy => policy.RequireClaim("Rank", "M3"));
                // 要求具有声明 Rank，且值为 P3 或 M3
                options.AddPolicy("RankClaimP3OrM3", policy => policy.RequireClaim("Rank", "P3", "M3"));
                // 要求具有声明 Rank，且同时具有两个值：P3 和 M3
                options.AddPolicy("RankClaimP3AndM3", policy => policy.RequireClaim("Rank", "P3").RequireClaim("Rank", "M3"));
                // 复杂声明
                options.AddPolicy("ComplexClaim", policy => policy.RequireAssertion(context =>
                    context.User.HasClaim(c => (c.Type == "Rank" || c.Type == "Name") && c.Issuer == "Issuer")));
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "XXTk.Auth.Samples.ClaimsBased.HttpApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "XXTk.Auth.Samples.ClaimsBased.HttpApi v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
