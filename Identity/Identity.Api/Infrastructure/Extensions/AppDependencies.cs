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
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["Masstransit:Host"], configuration["Masstransit:Virtualhost"], h =>
                {
                    h.Username(configuration["Masstransit:UserName"]);
                    h.Password(configuration["Masstransit:Password"]);
                });
            });
        });
    }
}