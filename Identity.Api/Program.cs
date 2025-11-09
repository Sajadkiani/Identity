using System;
using System.Reflection;
using System.Text;
using EventBus.MtuBus.Extensions;
using Identity.Api;
using Identity.Api.Application.Behaviors;
using Identity.Api.Application.DomainEventHandlers.Users;
using Identity.Api.Extensions;
using Identity.Api.Grpc;
using Identity.Api.Security;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using Identity.Domain.Validations.Users;
using Identity.Infrastructure.Clients.Grpc;
using Identity.Infrastructure.Dapper;
using Identity.Infrastructure.MtuBus;
using Identity.Infrastructure.MtuBus.Consumers;
using Identity.Infrastructure.Options;
using Identity.Infrastructure.ORM.BcValidations;
using Identity.Infrastructure.ORM.Dapper;
using Identity.Infrastructure.ORM.EF;
using Identity.Infrastructure.Utils;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var web = WebApplication.CreateBuilder(args);

AppOptions.ApplicationOptionContext.ConnectionString = web.Configuration.GetConnectionString("DefaultConnection");

//serilog configurations
web.Host.UseSerilog((ctx, config) =>
{
    config.Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
        .WriteTo.Console()
        .WriteTo.Elasticsearch(
            new ElasticsearchSinkOptions(new Uri(web.Configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{DateTime.UtcNow:yyyy-MM}"
            });

    config.ReadFrom.Configuration(ctx.Configuration);
});

web.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(web.Configuration.GetConnectionString("DefaultConnection"));
});

web.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = AppOptions.Jwt.Scheme;
    opt.DefaultScheme = AppOptions.Jwt.Scheme;
    opt.DefaultChallengeScheme = AppOptions.Jwt.Scheme;
}).AddScheme<AppOptions.Jwt, AppAuthenticationHandler>(AppOptions.Jwt.Scheme, opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = web.Configuration["Jwt:Issuer"],
        ValidAudience = web.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(web.Configuration["Jwt:Key"]))
    };
});

web.Services.AddAuthorization();
web.AddIntegrationEvents();
web.Services.AddAppOptions(web.Configuration);
web.Services.AddMemoryCache();
web.Services.AddGrpc(opt => { opt.Interceptors.Add<ExceptionInterceptor>(); });
web.Services.AddControllers();
web.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
web.Services.AddAppSwagger();


// Core services
web.Services.AddScoped<IPasswordService, PasswordService>();
web.Services.AddScoped<IEventInitializer, EventInitializer>();
web.Services.AddScoped<IAppRandoms, AppRandoms>();
web.Services.AddScoped<IQueryExecutor, DapperQueryExecutor>();
web.Services.AddScoped<ICurrentUser, CurrentUser>();
web.Services.AddScoped<IUserBcScopeValidation, UserBcScopeValidation>();
web.Services.AddScoped<IUserStore, Identity.Infrastructure.EF.Stores.UserStore>();

// Dapper context
web.Services.AddScoped(sp =>
    new DapperContext(web.Configuration.GetConnectionString("DefaultConnection")));

// Register pipeline behaviors
web.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
web.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
web.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

web.Services.AddMtuBus(web.Configuration);

web.AddExtraConfigs();
web.ConfigLogger();


var app = web.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();
app.UseAppSwagger();

app.UseAppProblemDetail();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<AuthGrpcService>();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

await app.RunAsync();


static class AppExtensions
{
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