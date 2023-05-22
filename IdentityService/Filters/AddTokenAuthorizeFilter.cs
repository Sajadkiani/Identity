﻿using System;
using IdentityService.Consts;
using IdentityService.Exceptions;
using IdentityService.Services;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Filters;

public class AppAuthorizeFilter : IAuthorizationFilter 
{
    private readonly IServiceProvider services;
    private readonly IMemoryCache cache;
    
    public AppAuthorizeFilter(
        IServiceProvider services,
        IMemoryCache cache
        )
    {
        this.services = services;
        this.cache = cache;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorization = context.HttpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorization))
        {
            return;
        }
        var referenceToken = authorization.Split(" ")[1];
        var token = cache.Get<AuthViewModel.GetTokenOutput>(CacheKeys.Token + referenceToken);
        if (token is null)
            throw new IdentityException.IdentityUnauthorizedException();
         
        context.HttpContext.Request.Headers.Authorization = token.AccessToken;
        SetCurrentUser(context.HttpContext);
        
    }
    
    private void SetCurrentUser(HttpContext context)
    {
        using var scope = services.CreateScope();
        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        currentUser.Set(context.User);
    }
}
