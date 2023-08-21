using Identity.Infrastructure.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.Api.Extensions;

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