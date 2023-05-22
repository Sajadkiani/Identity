using System;
using System.Threading.Tasks;
using IdentityService.Consts;
using IdentityService.Entities;
using IdentityService.Exceptions;
using IdentityService.Services;
using IdentityService.ViewModels;
using MassTransit.Caching;
using MassTransit.Internals.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityService.Security;

public class AppAuthorizationRequirement : IAuthorizationRequirement
{
    
}

public class AppAuthorizationHandler : AuthorizationHandler<AppAuthorizationRequirement>
{
    private readonly ILogger _logger;

    public AppAuthorizationHandler(ILoggerFactory loggerFactory)
        => _logger = loggerFactory.CreateLogger(GetType().FullName);
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AppAuthorizationRequirement requirement)
    {
        // var referenceToken = context.Request.Headers.Authorization.ToString().Split(" ")[1];
        // var token = cache.Get<AuthViewModel.GetTokenOutput>(CacheKeys.Token + referenceToken);
        // if (token is null)
        //     throw new IdentityException.IdentityUnauthorizedException();
        //
        // context.Request.Headers.Authorization = token.AccessToken;
        //
        // if (authorizeResult.Succeeded == false)
        // {
        //     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //     return;
        // }
        //
        // SetCurrentUser(context);

        var x = 1;
    }
}
//
// public class AppAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
// {
//     private readonly IServiceProvider services;
//     private readonly IMemoryCache cache;
//     private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
//
//     public AppAuthorizationMiddlewareResultHandler(
//         IServiceProvider services,
//         IMemoryCache cache)
//     {
//         this.services = services;
//         this.cache = cache;
//     }
//     
//     public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
//         PolicyAuthorizationResult authorizeResult)
//     {
//         var referenceToken = context.Request.Headers.Authorization.ToString().Split(" ")[1];
//         var token = cache.Get<AuthViewModel.GetTokenOutput>(CacheKeys.Token + referenceToken);
//         if (token is null)
//             throw new IdentityException.IdentityUnauthorizedException();
//         
//         context.Request.Headers.Authorization = token.AccessToken;
//         
//         if (authorizeResult.Succeeded == false)
//         {
//             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//             return;
//         }
//
//         SetCurrentUser(context);
//     }
//
//     private void SetCurrentUser(HttpContext context)
//     {
//         using var scope = services.CreateScope();
//         var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
//         currentUser.Set(context.User);
//     }
//     
// }