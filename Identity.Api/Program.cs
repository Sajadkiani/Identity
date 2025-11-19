using System;
using System.Reflection;
using System.Text;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using EventBus.MtuBus.Extensions;
using EventBus.Services;
using Identity.Api.Application.Behaviors;
using Identity.Api.Extensions;
using Identity.Api.Grpc;
using Identity.Api.MapperProfiles;
using Identity.Api.Security;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using Identity.Domain.Validations.Users;
using Identity.Infrastructure.Clients.Grpc;
using Identity.Infrastructure.Dapper;
using Identity.Infrastructure.Data.EF;
using Identity.Infrastructure.Options;
using Identity.Infrastructure.ORM.BcValidations;
using Identity.Infrastructure.ORM.Dapper;
using Identity.Infrastructure.ORM.EF.Stores;
using Identity.Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var web = WebApplication.CreateBuilder(args);

web.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(web.Configuration.GetConnectionString("DefaultConnection"));
});

web.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = AppOptions.Jwt.Scheme;
    opt.DefaultScheme = AppOptions.Jwt.Scheme;
    opt.DefaultChallengeScheme = AppOptions.Jwt.Scheme;
})
    .AddScheme<AppOptions.Jwt, AppAuthenticationHandler>(AppOptions.Jwt.Scheme, opt =>
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

web.Services.AddAutoMapper(expr =>
{
    expr.AddProfile<AutoMapperProfile>();
});

web.Services.AddAuthorization();
web.Services.AddAppOptions(web.Configuration);
web.Services.AddMemoryCache();
web.Services.AddGrpc(opt => { opt.Interceptors.Add<ExceptionInterceptor>(); });
web.Services.AddControllers();
web.Services.AddAppSwagger();


// Core services
web.Services.AddScoped<IPasswordService, PasswordService>();
web.Services.AddScoped<IEventInitializer, EventInitializer>();
web.Services.AddScoped<IAppRandoms, AppRandoms>();
web.Services.AddScoped<IQueryExecutor, DapperQueryExecutor>();
web.Services.AddScoped<ICurrentUser, CurrentUser>();
web.Services.AddScoped<IUserBcScopeValidation, UserBcScopeValidation>();
web.Services.AddScoped<IUserStore, UserStore>();

// Dapper context
web.Services.AddScoped(sp =>
    new DapperContext(web.Configuration.GetConnectionString("DefaultConnection")));

web.Services.AddMtuBus(web.Configuration);

// Register pipeline behaviors
web.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
web.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
web.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});


web.AddExtraConfigs();
web.ConfigLogger();


var app = web.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

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

    public static WebApplicationBuilder ConfigLogger(this WebApplicationBuilder builder)
    {
        var logOptions = builder.Configuration.GetSection("Logging").Get<AppOptions.LoggingOption>();
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .Enrich.WithProperty("Application", logOptions.ApplicationName)
            .WriteTo.Console(formatProvider: null, theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
            .WriteTo.Elasticsearch(new[] { new Uri(logOptions.ElasticUrl) }, opts =>
            {
                opts.DataStream = new DataStreamName("logs", logOptions.ApplicationName,
                    $"{builder.Environment.EnvironmentName}");
                opts.BootstrapMethod = BootstrapMethod.None;
            })
            .CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }
}

public partial class Program
{
    public static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}