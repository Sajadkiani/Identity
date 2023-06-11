using Autofac.Extensions.DependencyInjection;
using Identity.Api.Infrastructure.AppServices;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Infrastructure.Extensions;

public static class AppDependencies
{
    public static IServiceCollection AddAppDependencies(this IServiceCollection services)
    {
        #region security
        services.AddAuthorization();
        services.AddAutofac();
        services.AddScoped<ICurrentUser, CurrentUser>();
        #endregion
        
        return services;
    }
}