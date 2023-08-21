using System;
using System.Data.Common;
using Autofac.Extensions.DependencyInjection;
using Identity.Api.Application.IntegrationEvents;
using IntegrationEventLogEF.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Extensions;

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
            config.AddConsumers(typeof(Program));
            config.UsingRabbitMq((context, cfg) =>
            {
                if (string.IsNullOrEmpty(configuration["Masstransit:Host"]))
                {
                    throw new Exception("Masstransit:Host config in appSettings not found.");
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