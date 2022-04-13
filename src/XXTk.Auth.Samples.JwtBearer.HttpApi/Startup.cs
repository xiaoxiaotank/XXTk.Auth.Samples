using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
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
            services.AddDistributedMemoryCache();

            services.Configure<JwtOptions>(Configuration.GetSection(JwtOptions.Name));

            var jwtOptions = Configuration.GetSection(JwtOptions.Name).Get<JwtOptions>();

            // ʹ�öԳƼ��ܽ���ǩ�������Ƽ���
            services.AddSingleton(sp => new SigningCredentials(jwtOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature));

            // ʹ�÷ǶԳƼ��ܽ���ǩ�����Ƽ���
            //var rsaSecurityPrivateKeyString = File.ReadAllText(Path.Combine(Env.ContentRootPath, "Rsa", "key.private.json"));
            //var rsaSecurityPublicKeyString = File.ReadAllText(Path.Combine(Env.ContentRootPath, "Rsa", "key.public.json"));
            //RsaSecurityKey rsaSecurityPrivateKey = new(JsonConvert.DeserializeObject<RSAParameters>(rsaSecurityPrivateKeyString));
            //RsaSecurityKey rsaSecurityPublicKey = new(JsonConvert.DeserializeObject<RSAParameters>(rsaSecurityPublicKeyString));
            //services.AddSingleton(sp => new SigningCredentials(rsaSecurityPrivateKey, SecurityAlgorithms.RsaSha256Signature));

            services.AddScoped<AppJwtBearerEvents>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // ��Ч��ǩ���㷨�б����г����㷨����Ч�ģ�ǿ�ҽ��鲻Ҫ���ø����ԣ�
                        // Ĭ�� null���������㷨����
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Aes128CbcHmacSha256 },
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
                        IssuerSigningKey = jwtOptions.SymmetricSecurityKey,
                        // �ǶԳƼ��ܣ�ʹ��˽Կ��ǩ��ʹ�ù�Կ��ǩ
                        //IssuerSigningKey = rsaSecurityPublicKey,
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

                        // token������Կ
                        //TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Total Bytes Length At Least 256!"))
                    };

                    // ��token��֤ͨ����ִ���� JwtBearerEvents.TokenValidated �󣩣�
                    // �Ƿ�token�洢�� Microsoft.AspNetCore.Authentication.AuthenticationProperties ��
                    // Ĭ�� true
                    options.SaveToken = true;

                    // token��֤�������б�
                    // Ĭ�� ��1�� JwtSecurityTokenHandler
                    //options.SecurityTokenValidators.Clear();
                    //options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());
                    // ͨ��Post���AppJwtSecurityTokenHandler

                    // ���ڣ�ָ��token�Ƿ������ĸ�Ⱥ��ģ�Ⱥ�巶Χ���������token���������Ȩ�޵���Դ����һ�飨��Դ��uri��
                    // ��������Ҫ���ڵ��䲻Ϊ�գ��� TokenValidationParameters.ValidAudience Ϊ��ʱ�����丳ֵ�� TokenValidationParameters.ValidAudience
                    // һ�㲻ʹ�ø�����
                    //options.Audience = jwtOptions.Audience;    

                    options.EventsType = typeof(AppJwtBearerEvents);
                });

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, AppJwtBearerPostConfigureOptions>());

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "XXTk.Auth.Samples.JwtBearer.HttpApi", Version = "v1" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };
                c.AddSecurityRequirement(securityRequirement);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // ����չʾ���ܡ�ǩ��ʱ����ϸ������Ϣ
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "XXTk.Auth.Samples.JwtBearer.HttpApi v1"));
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
