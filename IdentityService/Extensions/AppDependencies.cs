using IdentityService.Services;
using IdentityService.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Extensions;

public static class AppDependencies
{
    public static IServiceCollection AddAppDependencies(this IServiceCollection services)
    {
        //TODO: add auto injector
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITokenGeneratorService, JwtTokenGeneratorService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAppRandoms, AppRandoms>();
        return services;
    }
}