using System;
using System.Data.Common;
using Autofac.Extensions.DependencyInjection;
using Identity.Api.Application.IntegrationEvents;
using Identity.Api.Infrastructure.Brokers;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.SeedWork;
using Identity.Infrastructure.Dapper;
using IntegrationEventLogEF.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserStore = Identity.Infrastructure.EF.Stores.UserStore;

namespace Identity.Api.Infrastructure.Extensions;

public static class AppDependencies
{
    public static void AddAppDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        #region security
        services.AddAuthorization();
        services.AddAutofac();
        services.AddIntegrationServices(configuration);
        #endregion
    }

    private static void AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));
        
        services.AddScoped<IIntegrationEventService, IntegrationEventService>();
        
        services.AddMassTransit(config =>
        {
            config.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter(false));
            config.UsingRabbitMq((context, cfg) =>
            {
                if (string.IsNullOrEmpty(configuration["Masstransit:Host"]))
                {
                    throw new Exception("Masstransit:Host config in appsettings not found.");
                }

                cfg.Host(configuration["Masstransit:Host"], configuration["Masstransit:Virtualhost"],
                    configuration["Masstransit:Port"], h =>
                    {
                        h.Username(configuration["Masstransit:UserName"]);
                        h.Password(configuration["Masstransit:Password"]);
                
                        //https://masstransit.io/documentation/configuration/transports/rabbitmq#configurebatchpublish
                        h.ConfigureBatchPublish(b =>
                        {
                            b.Enabled = true;
                            b.Timeout = TimeSpan.FromMilliseconds(2);
                        });
                    });
                
                
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}