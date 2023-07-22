using System.Threading.Tasks;
using AutoMapper;
using EventBus.Abstractions;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Application.Queries.Users;
using Identity.Api.Infrastructure.Brokers;
using Identity.Api.Infrastructure.Consts;
using Identity.Api.Infrastructure.Options;
using Identity.Api.Infrastructure.Services;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Identity.Api.Controllers;

[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenGeneratorService tokenGenerator;
    private readonly IMapper mapper;
    private readonly IMemoryCache cache;
    private readonly AppOptions.Jwt jwt;
    private readonly IEventBus eventHandler;
    private readonly ILogger<AuthController> logger;

    public AuthController(
        ITokenGeneratorService tokenGenerator,
        IMapper mapper,
        IMemoryCache cache,
        AppOptions.Jwt jwt,
        IEventBus eventHandler,
        ILogger<AuthController> logger
    )
    {
        this.tokenGenerator = tokenGenerator;
        this.mapper = mapper;
        this.cache = cache;
        this.jwt = jwt;
        this.eventHandler = eventHandler;
        this.logger = logger;
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
        return await eventHandler.SendMediator(new RefreshTokenQuery { RefreshToken = input.RefreshToken});
    }
    
    private async Task<AuthViewModel.GetTokenOutput> GetTokenAsync(User user)
    {
        var token = await tokenGenerator.GenerateTokenAsync(user);
        await SaveTokenAsync(token, user);
        return token;
    }

    private async Task SaveTokenAsync(AuthViewModel.GetTokenOutput token, User user)
    {
        await eventHandler.SendMediator(new AddTokenCommand{AccessToken = token.AccessToken, ExpireDate = token.ExpireDate,
            RefreshToken = token.RefreshToken, User = user, UserId = user.Id});

        cache.Set(CacheKeys.Token + token.RefreshToken, token,
            token.ExpireDate.AddMinutes(jwt.DurationInMinutesRefresh));
    }
}