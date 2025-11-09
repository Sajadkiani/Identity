using System;
using Identity.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Filters;

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
        
        SetCurrentUser(context.HttpContext);
    }
    
    private void SetCurrentUser(HttpContext context)
    {
        using var scope = services.CreateScope();
        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        currentUser.Set(context.User);
    }
}
