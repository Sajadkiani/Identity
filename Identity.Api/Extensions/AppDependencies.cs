using System;
using System.Data.Common;
using EventBus.Abstractions;
using Identity.Api.Application.DomainEventHandlers.Users;
using Identity.Api.Application.IntegrationEvents;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using Identity.Domain.Validations.Users;
using Identity.Infrastructure.Dapper;
using Identity.Infrastructure.EF.Stores;
using Identity.Infrastructure.ORM.BcValidations;
using Identity.Infrastructure.Utils;
using IntegrationEventLogEF.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Extensions;

public static class AppDependencies
{
    public static void AddAppDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        #region security
        services.AddAuthorization();
        services.AddIntegrationServices(configuration);
        #endregion
    }

    private static void AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));
        
        services.AddScoped<IIntegrationEventService, IntegrationEventService>();
        
        // services.AddMassTransit(config =>
        // {
        //     config.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter(false));
        //     config.AddConsumers(typeof(Program));
        //     config.UsingRabbitMq((context, cfg) =>
        //     {
        //         if (string.IsNullOrEmpty(configuration["Masstransit:Host"]))
        //         {
        //             throw new Exception("Masstransit:Host config in appSettings not found.");
        //         }
        //
        //         cfg.Host(configuration["Masstransit:Host"], configuration["Masstransit:Virtualhost"],
        //             configuration["Masstransit:Port"], h =>
        //             {
        //                 h.Username(configuration["Masstransit:UserName"]);
        //                 h.Password(configuration["Masstransit:Password"]);
        //         
        //                 //https://masstransit.io/documentation/configuration/transports/rabbitmq#configurebatchpublish
        //                 h.ConfigureBatchPublish(b =>
        //                 {
        //                     b.Enabled = true;
        //                     b.Timeout = TimeSpan.FromMilliseconds(2);
        //                 });
        //             });
        //         
        //         
        //         cfg.ConfigureEndpoints(context);
        //     });
        // });
    }
}
public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services, string dapperConnectionString)
    {
        // Core services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IEventInitializer, EventInitializer>();
        services.AddScoped<IAppRandoms, AppRandoms>();
        services.AddScoped<IEventBus, Identity.Infrastructure.Brokers.EventBus>();
        services.AddScoped<IQueryExecutor, DapperQueryExecutor>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        // Dapper context
        services.AddScoped(sp => new DapperContext(dapperConnectionString));

        // Register all *Store classes as their implemented interfaces
        // services.Scan(scan => scan
        //     .FromAssemblies(typeof(IUserStore).Assembly, typeof(UserStore).Assembly)
        //     .AddClasses(c => c.Where(t => t.Name.EndsWith("Store")))
        //     .AsImplementedInterfaces()
        //     .WithScopedLifetime());
        //
        // // Register all *Validation classes as their implemented interfaces
        // services.Scan(scan => scan
        //     .FromAssemblies(typeof(IUserBcScopeValidation).Assembly, typeof(UserBcScopeValidation).Assembly)
        //     .AddClasses(c => c.Where(t => t.Name.EndsWith("Validation")))
        //     .AsImplementedInterfaces()
        //     .WithScopedLifetime());

        // Register MediatR domain event handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(TestDomainEventHandler).Assembly);
        });

        return services;
    }
}
