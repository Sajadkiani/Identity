using Autofac;
using Autofac.Extensions.DependencyInjection;
using Identity.Api;
using Identity.Api.Infrastructure.AutofacModules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


var webApplicationBuilder = WebApplication.CreateBuilder(args);
var host = webApplicationBuilder.Host;
webApplicationBuilder.Services.ConfigureServices(webApplicationBuilder.Environment,
    webApplicationBuilder.Configuration);

AddAutofacRequirements(host, webApplicationBuilder.Configuration);
AddExtraConfigs(host, webApplicationBuilder.Environment);

var app = webApplicationBuilder.Build();
app.Configure(webApplicationBuilder.Environment);
await app.RunAsync();



static void AddAutofacRequirements(IHostBuilder builder, ConfigurationManager configurationManager)
{
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