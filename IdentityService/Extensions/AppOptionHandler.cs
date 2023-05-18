using IdentityService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Extensions;

public static class AppOptionHandler
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppOptions.Jwt>(
            configuration.GetSection(AppOptions.Jwt.Section))
            .AddSingleton<AppOptions.Jwt>();
        
        return services;
    }
    
}