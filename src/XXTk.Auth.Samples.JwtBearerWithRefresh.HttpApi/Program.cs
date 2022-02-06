using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace XXTk.Auth.Samples.JwtBearerWithRefresh.HttpApi
{
    /// <summary>
    /// ����Ŀ��ʾ�����Զ�����Ļ���Jwt����֤����
    /// ����Ҫ�鿴����ʵ�֣���ο���Ŀ��XXTk.Auth.Samples.JwtBearer.HttpApi
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
