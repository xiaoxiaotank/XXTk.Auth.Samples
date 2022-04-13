using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;
using XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi
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

            // 当Handler中依赖其他服务时，注意生命周期提升的问题
            services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, MinimumAgeAuthorizationHandler>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, MinimumAgeAnotherAuthorizationHandler>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, MultiRequirementsAuthorizationHandler>());

            services.AddTransient<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();

            services.AddTransient<IAuthorizationMiddlewareResultHandler, MyAuthorizationMiddlewareResultHandler>();

            services.AddAuthorization(options =>
            {
                //options.InvokeHandlersAfterFailure = false;
                options.AddPolicy("AtLeast18Age", policy => policy.RequireMinimumAge(18));
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "XXTk.Auth.Samples.PolicyBased.HttpApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "XXTk.Auth.Samples.PolicyBased.HttpApi v1"));
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
