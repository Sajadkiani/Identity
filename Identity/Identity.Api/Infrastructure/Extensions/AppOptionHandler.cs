using System;
using IdentityService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityService.Extensions;

public static class AppOptionHandler
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        // var provider = services.BuildServiceProvider();
        services.Configure<AppOptions.Jwt>(configuration.GetSection(AppOptions.Jwt.Section));
        services.AddSingleton(provider =>
            provider.GetService<IOptionsMonitor<AppOptions.Jwt>>().CurrentValue
        );
        
        
        return services;
    }
    
}