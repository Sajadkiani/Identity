using IdentityService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Extensions;

public static class AppDependencies
{
    public static IServiceCollection AddAppDependencies(this IServiceCollection services)
    {
        //TODO: add auto injector
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        return services;
    }
}