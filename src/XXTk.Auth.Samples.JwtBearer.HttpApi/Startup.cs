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
using System.IO;
using XXTk.Auth.Samples.JwtBearer.HttpApi.Authentication.JwtBearer;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var jwtOptions = Configuration.GetSection(JwtOptions.Name).Get<JwtOptions>();

            #region Rsa�������
            jwtOptions.RsaSecurityPrivateKeyString = File.ReadAllText(Path.Combine(Env.ContentRootPath, "Rsa", "key.private.json"));
            jwtOptions.RsaSecurityPublicKeyString = File.ReadAllText(Path.Combine(Env.ContentRootPath, "Rsa", "key.public.json"));
            jwtOptions.Algorithms = SecurityAlgorithms.RsaSha256Signature;
            #endregion

            services.AddSingleton(jwtOptions);

            // ʹ�öԳƼ��ܽ���ǩ�������Ƽ���
            //services.AddSingleton(sp => new SigningCredentials(jwtOptions.SymmetricSecurityKey, jwtOptions.Algorithms));

            // ʹ�÷ǶԳƼ��ܽ���ǩ�����Ƽ���
            services.AddSingleton(sp => new SigningCredentials(jwtOptions.RsaSecurityPrivateKey, jwtOptions.Algorithms));

            services.AddScoped<AppJwtBearerEvents>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // ��Ч��ǩ���㷨�б����г����㷨����Ч��
                        // Ĭ�� null���������㷨����
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256 },
                        // ��Ч��token����
                        // Ĭ�� null�����������;���
                        ValidTypes = new[] { JwtConstants.HeaderType },

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
                        // �ԳƼ��ܣ�ʹ����ͬ��key���м�ǩ��ǩ
                        //IssuerSigningKey = jwtOptions.SymmetricSecurityKey,
                        // �ǶԳƼ��ܣ�ʹ��˽Կ��ǩ��ʹ�ù�Կ��ǩ
                        IssuerSigningKey = jwtOptions.RsaSecurityPublicKey,
                        // �Ƿ���֤ǩ��������tokenǩ������Կ
                        // Ĭ�� false
                        ValidateIssuerSigningKey = true,

                        // �Ƿ���֤token�Ƿ�����Ч���ڣ�����nbf��exp��
                        // Ĭ�� true
                        ValidateLifetime = true,

                        // �Ƿ�Ҫ��token�������ǩ��
                        // Ĭ�� true
                        RequireSignedTokens = true,
                        // �Ƿ�Ҫ��token�����������ʱ��
                        // Ĭ�� true
                        RequireExpirationTime = true,

                        // ���� HttpContext.User.Identity.NameClaimType������ HttpContext.User.Identity.Name ȡ����ȷ��ֵ
                        NameClaimType = JwtClaimTypes.Name,
                        // ���� HttpContext.User.Identity.RoleClaimType������ HttpContext.User.Identity.IsInRole(xxx) ȡ����ȷ��ֵ
                        RoleClaimType = JwtClaimTypes.Role,

                        // ʱ��Ư��
                        // ��������֤token��Ч��ʱ������һ����ʱ������ʱ��մﵽtoken��exp����������δ��5�����ڸ�token��Ȼ��Ч��
                        // Ĭ�� 300s���� 5min
                        ClockSkew = TimeSpan.Zero,
                    };

                    // ��token��֤ͨ����ִ���� JwtBearerEvents.TokenValidated �󣩣�
                    // �Ƿ�token�洢�� Microsoft.AspNetCore.Authentication.AuthenticationProperties ��
                    // Ĭ�� true
                    options.SaveToken = true;

                    // token��֤�������б�
                    // Ĭ�� ��1�� JwtSecurityTokenHandler
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());

                    // ���ڣ�ָ��token�Ƿ������ĸ�Ⱥ��ģ�Ⱥ�巶Χ���������token���������Ȩ�޵���Դ����һ�飨��Դ��uri��
                    // ��������Ҫ���ڵ��䲻Ϊ�գ��� TokenValidationParameters.ValidAudience Ϊ��ʱ�����丳ֵ�� TokenValidationParameters.ValidAudience
                    // һ�㲻ʹ�ø�����
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
