using System;
using System.Collections.Generic;
using System.Security.Claims;
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
using Microsoft.Extensions.Options;

namespace IdentityService.Security;

// public class AuthorizationService : IAuthorizationService
// {
//     private readonly IAuthorizationHandlerProvider _handlerProvider;
//     private readonly IAuthorizationHandlerContextFactory _contextFactory;
//     private readonly IAuthorizationEvaluator _evaluator;
//
//     public AuthorizationService(IAuthorizationPolicyProvider policyProvider,
//         IAuthorizationHandlerProvider handlerProvider, ILogger<DefaultAuthorizationService> logger,
//         IAuthorizationHandlerContextFactory contextFactory, IAuthorizationEvaluator evaluator,
//         IOptions<AuthorizationOptions> options)
//     {
//         _handlerProvider = handlerProvider;
//         _contextFactory = contextFactory;
//         _evaluator = evaluator;
//     }
//     public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
//     {
//         var authContext = _contextFactory.CreateContext(requirements, user, resource);
//
//         // By default this returns an IEnumerable<IAuthorizationHandlers> from DI.
//         var handlers = await _handlerProvider.GetHandlersAsync(authContext);
//
//         // Invoke all handlers.
//         foreach (var handler in handlers)
//         {
//             await handler.HandleAsync(authContext);
//         }
//
//         // Check the context, by default success is when all requirements have been met.
//         return _evaluator.Evaluate(authContext);
//     }
//
//     public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
//     {
//         throw new NotImplementedException();
//     }
// }

public class AppAuthorizationRequirement : IAuthorizationRequirement
{
    
}

public class AppAuthorizationHandler : IAuthorizationHandler
{
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IMemoryCache cache;
    private readonly IServiceProvider provider;
    private readonly ILogger _logger;

    public AppAuthorizationHandler(ILoggerFactory loggerFactory, IHttpContextAccessor contextAccessor,
        IMemoryCache cache, IServiceProvider provider)
    {
        this.contextAccessor = contextAccessor;
        this.cache = cache;
        this.provider = provider;
        _logger = loggerFactory.CreateLogger(GetType().FullName);
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var authorization = contextAccessor.HttpContext!.Request.Headers.Authorization;
        if (string.IsNullOrWhiteSpace(authorization.ToString()))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var referenceToken = authorization.ToString().Split(" ")[1];
        var token = cache.Get<AuthViewModel.GetTokenOutput>(CacheKeys.Token + referenceToken);
        if (token is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        contextAccessor.HttpContext!.Request.Headers.Authorization = $"{authorization.ToString().Split(" ")[0]} {token.AccessToken}";
        SetCurrentUser(contextAccessor.HttpContext);
        
        context.Succeed(new AppAuthorizationRequirement());
        return Task.CompletedTask;
    }

    private void SetCurrentUser(HttpContext context)
    {
        using var scope = provider.CreateScope();
        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        currentUser.Set(context.User);
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