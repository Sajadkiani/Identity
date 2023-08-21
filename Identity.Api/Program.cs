using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Identity.Api;
using Identity.Api.AutofacModules;
using Identity.Api.Infrastructure.AutofacModules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

//serilog configurations
webApplicationBuilder.Host.UseSerilog((ctx, config) =>  
{
    config.Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
       .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
       .WriteTo.Console()
       .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(webApplicationBuilder.Configuration["ElasticConfiguration:Uri"]))  
                {  
                    AutoRegisterTemplate = true,  
                    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{DateTime.UtcNow:yyyy-MM}"  
                });
    config.ReadFrom.Configuration(ctx.Configuration);
});

var host = webApplicationBuilder.Host;
webApplicationBuilder.Services.ConfigureServices(webApplicationBuilder.Environment,
    webApplicationBuilder.Configuration);

AddAutofacRequirements(host, webApplicationBuilder);
AddExtraConfigs(host, webApplicationBuilder.Environment);

ConfigLogger(webApplicationBuilder);

var app = webApplicationBuilder.Build();
app.Configure(webApplicationBuilder.Environment);


try
{
    await app.RunAsync();
}
catch (System.Exception ex)
{
    Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
    throw;
}

//method and class 
static void AddAutofacRequirements(IHostBuilder builder, WebApplicationBuilder webApplicationBuilder)
{
    var configurationManager = webApplicationBuilder.Configuration;
    builder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureContainer<ContainerBuilder>(conbuilder =>
            conbuilder.RegisterModule(
                new ApplicationModule(configurationManager.GetConnectionString("DapperConnectionString"))))
        
        .ConfigureContainer<ContainerBuilder>(conbuilder =>
            conbuilder.RegisterModule(new MediatorModule()));
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
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(webApplicationBuilder.Configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{webApplicationBuilder.Environment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
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
