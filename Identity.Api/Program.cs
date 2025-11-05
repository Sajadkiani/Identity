using System;
using System.Reflection;
using Identity.Api;
using Identity.Api.Application.DomainEventHandlers.Users;
using Identity.Api.Extensions.Options;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using Identity.Infrastructure.Dapper;
using Identity.Infrastructure.MtuBus;
using Identity.Infrastructure.MtuBus.Consumers;
using Identity.Infrastructure.ORM.Dapper;
using Identity.Infrastructure.ORM.EF;
using Identity.Infrastructure.Utils;
using IntegrationEventLogEF.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
});

// Core services
webApplicationBuilder.Services.AddScoped<IPasswordService, PasswordService>();
webApplicationBuilder.Services.AddScoped<IEventInitializer, EventInitializer>();
webApplicationBuilder.Services.AddScoped<IAppRandoms, AppRandoms>();
webApplicationBuilder.Services.AddScoped<IQueryExecutor, DapperQueryExecutor>();
webApplicationBuilder.Services.AddScoped<ICurrentUser, CurrentUser>();
webApplicationBuilder.Services.AddScoped<IUserStore, Identity.Infrastructure.EF.Stores.UserStore>();

// Dapper context
webApplicationBuilder.Services.AddScoped(sp => new DapperContext(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection")));
webApplicationBuilder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(TestDomainEventHandler).Assembly);
});webApplicationBuilder.AddMtuBus();

webApplicationBuilder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

webApplicationBuilder.Services.ConfigureServices(webApplicationBuilder.Environment,
    webApplicationBuilder.Configuration);

webApplicationBuilder.AddExtraConfigs();
webApplicationBuilder.ConfigLogger();



var app = webApplicationBuilder.Build();
app.Configure(webApplicationBuilder.Environment);
await app.RunAsync();









static class AppExtensions
{
    public static WebApplicationBuilder AddMtuBus(this WebApplicationBuilder web)
    {
        web.Services.Configure<AppOptions.MtuRabbitMqOptions>(web.Configuration.GetSection("Rabbitmq"));

        web.Services.AddSingleton<IMtuBusConnectionManager, MtuBusConnectionManager>();
        web.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        web.Services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>(opt =>
        {
            var logger = opt.GetRequiredService<ILogger<IntegrationEventDispatcher>>();
            var options = opt.GetRequiredService<IOptions<AppOptions.MtuRabbitMqOptions>>();

            return IntegrationEventDispatcher.CreateAsync(options, logger).GetAwaiter().GetResult();
        });
        
        web.Services.AddScoped<IMtuConsumer, TestConsumer>();

        return web;
    }
    
    public static WebApplicationBuilder AddExtraConfigs(this WebApplicationBuilder web)
    {
        web.Host.ConfigureAppConfiguration(conf =>
        {
            conf.AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{web.Environment.EnvironmentName}.json", optional: true);
        });

        return web;
    }
    
    public static WebApplicationBuilder ConfigLogger(this WebApplicationBuilder webApplicationBuilder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(webApplicationBuilder.Configuration["ElasticConfiguration:Uri"]))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat =
                        $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{webApplicationBuilder.Environment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                })
            .Enrich.WithProperty("Environment", webApplicationBuilder.Environment.EnvironmentName)
            .ReadFrom.Configuration(webApplicationBuilder.Configuration)
            .CreateLogger();

        return webApplicationBuilder;
    }
}

public partial class Program
{
    public static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}