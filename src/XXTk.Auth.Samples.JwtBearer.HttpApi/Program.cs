using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace XXTk.Auth.Samples.JwtBearer.HttpApi
{
    /// <summary>
    /// 该项目演示最基础的基于Jwt的认证方案
    /// 若想要实现自动续租功能，请参考项目：XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;

                    config.AddJsonFile("jwt.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"jwt.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                });
    }
}
