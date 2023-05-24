using System.Collections.Generic;
using IdentityService.Options;
using IdentityService.Security;
using IdentityService.Services;
using IdentityService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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

        #region security
        services.AddAuthorization();
        services.AddScoped<ICurrentUser, CurrentUser>();
        #endregion
        
        return services;
    }
}