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
using Identity.Infrastructure.ORM.Dapper;
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
    }
}
