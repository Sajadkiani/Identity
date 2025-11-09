using System;
using System.Data.Common;
using System.Reflection;
using Identity.Api.Application.IntegrationEvents;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Extensions;

public static class AppDependencies
{
    public static WebApplicationBuilder AddIntegrationEvents(this WebApplicationBuilder web)
    {
        web.Services.AddDbContext<IntegrationEventLogContext>(options =>
        {
            options.UseSqlServer(web.Configuration["DefaultConnection"],
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        });
        
        web.Services.AddScoped<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));
        
        web.Services.AddScoped<IIntegrationEventService, IntegrationEventService>();

        return web;
    }
}
