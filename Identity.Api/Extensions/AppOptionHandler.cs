using Identity.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.Api.Extensions;

public static class AppOptionHandler
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppOptions.Jwt>(configuration.GetSection(AppOptions.Jwt.Section));
        services.AddSingleton(provider =>
            provider.GetService<IOptionsMonitor<AppOptions.Jwt>>().CurrentValue
        );

        services.Configure<AppOptions.LoggingOption>(configuration.GetSection("Logging"));
        return services;
    }
}