using System.Threading.Tasks;
using AutoMapper;
using Identity.Api.Infrastructure.AppServices;
using Identity.Api.Infrastructure.Brokers;
using Identity.Api.Infrastructure.Consts;
using Identity.Api.Infrastructure.Exceptions;
using Identity.Domain.Aggregates.Users;
using IdentityService.Api.Application.Commands.Users;
using IdentityService.Consts;
using IdentityService.Options;
using IdentityService.Services;
using IdentityService.ViewModels;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityService.Api.Controllers;

[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ITokenGeneratorService tokenGenerator;
    private readonly ITokenService tokenService;
    private readonly IMapper mapper;
    private readonly IMemoryCache cache;
    private readonly AppOptions.Jwt jwt;
    private readonly IEventHandler eventHandler;

    public AuthController(
        IUserService userService,
        ITokenGeneratorService tokenGenerator,
        ITokenService tokenService,
        IMapper mapper,
        IMemoryCache cache,
        AppOptions.Jwt jwt,
        IEventHandler eventHandler
    )
    {
        this.userService = userService;
        this.tokenGenerator = tokenGenerator;
        this.tokenService = tokenService;
        this.mapper = mapper;
        this.cache = cache;
        this.jwt = jwt;
        this.eventHandler = eventHandler;
    }
    
    [HttpPost("login")]
    public async Task<AuthViewModel.GetTokenOutput> LoginAsync(AuthViewModel.LoginInput input)
    {
        return await eventHandler.SendMediator(new LoginCommand(input.UserName,
            input.Password));
    }

    [HttpPost("refresh")]
    public async Task<AuthViewModel.GetTokenOutput> RefreshTokenAsync(AuthViewModel.RefreshTokenInput input)
    {
        return await eventHandler.SendMediator(new RefreshTokenQuery{input})
        
        
        var token = await tokenService.GetTokenByRefreshAsync(input.RefreshToken);
        if (token is null)
            throw new IdentityException.IdentityInternalException(AppMessages.UserNotFound);

        var user = await userService.GetUserAsync(token.UserId);
        if (user is null)
            throw new IdentityException.IdentityInternalException(AppMessages.UserNotFound);
        
        return await GetTokenAsync(user);
    }
    
    private async Task<AuthViewModel.GetTokenOutput> GetTokenAsync(User user)
    {
        var token = await tokenGenerator.GenerateTokenAsync(user);
        await SaveTokenAsync(token, user);
        return token;
    }

    private async Task SaveTokenAsync(AuthViewModel.GetTokenOutput token, User user)
    {
        var tokenInput = mapper.Map<AuthViewModel.AddTokenInput>(token);
        tokenInput.UserId = user.Id;

        await tokenService.AddTokenAsync(tokenInput);

        cache.Set(CacheKeys.Token + token.RefreshToken, token,
            tokenInput.ExpireDate.AddMinutes(jwt.DurationInMinutesRefresh));
    }
}