using System;
using System.Linq;
using System.Reflection;
using Identity.Api;
using Identity.Api.Extensions.Options;
using Identity.Infrastructure.MtuBus;
using Identity.Infrastructure.MtuBus.Consumers;
using Identity.Infrastructure.ORM.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
});

//serilog configurations
webApplicationBuilder.Host.UseSerilog((ctx, config) =>
{
    config.Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
        .WriteTo.Console()
        .WriteTo.Elasticsearch(
            new ElasticsearchSinkOptions(new Uri(webApplicationBuilder.Configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{DateTime.UtcNow:yyyy-MM}"
            });

    config.ReadFrom.Configuration(ctx.Configuration);
});


webApplicationBuilder.Services.Configure<AppOptions.RabbitMqOptions>(webApplicationBuilder.Configuration.GetSection("Rabbitmq"));
webApplicationBuilder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
webApplicationBuilder.Services.AddSingleton<IIntegrationEventDispatcher, EventPublisher>(opt =>
{
    var rabbitmqOption = opt.GetRequiredService<IOptions<AppOptions.RabbitMqOptions>>();
    var logger = opt.GetRequiredService<ILogger<EventPublisher>>();

    return EventPublisher.CreateAsync(rabbitmqOption, logger).GetAwaiter().GetResult();
});

webApplicationBuilder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

var host = webApplicationBuilder.Host;
webApplicationBuilder.Services.ConfigureServices(webApplicationBuilder.Environment,
    webApplicationBuilder.Configuration);

AddExtraConfigs(host, webApplicationBuilder.Environment);

ConfigLogger(webApplicationBuilder);

var app = webApplicationBuilder.Build();
app.Configure(webApplicationBuilder.Environment);

await app.RunAsync();


//method and class 
// static void AddAutofacRequirements(IHostBuilder builder, WebApplicationBuilder webApplicationBuilder)
// {
//     var configurationManager = webApplicationBuilder.Configuration;
//     builder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
//         .ConfigureContainer<ContainerBuilder>(conbuilder =>
//             conbuilder.RegisterModule(
//                 new ApplicationModule(configurationManager.GetConnectionString("DapperConnectionString"))))
//         
//         .ConfigureContainer<ContainerBuilder>(conbuilder =>
//             conbuilder.RegisterModule(new MediatorModule()));
// }


public static class MtuBusServiceCollectionExtensions
{
    public static WebApplicationBuilder AddMtuBus<T>(this WebApplicationBuilder web, IConfiguration configuration)
    {
        web.Services.Configure<AppOptions.MTuRabbitMqOptions>(
            configuration.GetSection("Rabbitmq"));

        web.Services.AddSingleton<IMtuBusConnectionManager, MtuBusConnectionManager>();

        var consumerTypes = typeof(T).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract &&
                        t.IsAssignableTo(typeof(BackgroundService)) &&
                        t.BaseType is { IsGenericType: true } &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(MTUConsumer<>));

        foreach (var consumerType in consumerTypes)
            web.Services.AddHostedService(consumerType);

        return web;
    }
}

static void AddExtraConfigs(IHostBuilder builder, IWebHostEnvironment webHostEnvironment)
{
    builder.ConfigureAppConfiguration(conf =>
    {
        conf.AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{webHostEnvironment.EnvironmentName}.json", optional: true);
    });
}

static void ConfigLogger(WebApplicationBuilder webApplicationBuilder)
{
    Console.WriteLine("myelastic:" + webApplicationBuilder.Configuration["ElasticConfiguration:Uri"]);
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
}

public partial class Program
{
    public static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}