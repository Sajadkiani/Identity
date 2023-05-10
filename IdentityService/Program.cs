using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var host = builder.Build();
            AddExtraConfigs(host, builder);
            host.Run();
        }

        private static void AddExtraConfigs(IHost host, IHostBuilder builder)
        {
            var env = host.Services.GetRequiredService<IWebHostEnvironment>();
            builder.ConfigureAppConfiguration(conf =>
            {
                conf.AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{env}.json", optional: true);
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
