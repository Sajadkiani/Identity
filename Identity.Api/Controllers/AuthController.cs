using System.Threading.Tasks;
using AutoMapper;
using EventBus.Abstractions;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Application.Queries.Users;
using Identity.Api.ViewModels;
using Identity.Infrastructure.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Identity.Api.Controllers;

[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IMemoryCache cache;
    private readonly AppOptions.Jwt jwt;
    private readonly IEventBus eventHandler;
    private readonly ILogger<AuthController> logger;

    public AuthController(
        IMapper mapper,
        IMemoryCache cache,
        AppOptions.Jwt jwt,
        IEventBus eventHandler,
        ILogger<AuthController> logger
    )
    {
        this.mapper = mapper;
        this.cache = cache;
        this.jwt = jwt;
        this.eventHandler = eventHandler;
        this.logger = logger;
    }

    [HttpPost("login")]
    public async Task<AuthViewModel.GetTokenOutput> LoginAsync([FromBody] AuthViewModel.LoginInput input)
    {
        return await eventHandler.SendMediator(new LoginCommand(input.UserName, input.Password, doHashPassword: true));
    }

    [HttpPost("refresh")]
    public async Task<AuthViewModel.GetTokenOutput> RefreshTokenAsync([FromBody] AuthViewModel.RefreshTokenInput input)
    {
        return await eventHandler.SendMediator(new RefreshTokenQuery { RefreshToken = input.RefreshToken});
    }
}