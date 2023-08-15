using System.Threading.Tasks;
using AutoMapper;
using EventBus.Abstractions;
using Grpc.Net.Client;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Application.Queries.Users;
using Identity.Api.Infrastructure.Extensions.Options;
using Identity.Api.Infrastructure.Services;
using Identity.Api.ViewModels;
using IdentityGrpcClient;
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

    [HttpGet("grpc")]
    public async Task grpc()
    {
        var message = new HelloRequest
        {
            Name = "Jaydeep"
        };
        var channel = GrpcChannel.ForAddress("https://localhost:5016");
        var client = new IdentityGrpcClient.TestGrpc.TestGrpcClient(channel);
        var srerveReply = await client.SayHelloAsync(message);
    }

    [HttpPost("login")]
    public async Task<AuthViewModel.GetTokenOutput> LoginAsync([FromBody] AuthViewModel.LoginInput input)
    {
        return await eventHandler.SendMediator(new LoginCommand(input.UserName,
            input.Password));
    }

    [HttpPost("refresh")]
    public async Task<AuthViewModel.GetTokenOutput> RefreshTokenAsync([FromBody] AuthViewModel.RefreshTokenInput input)
    {
        return await eventHandler.SendMediator(new RefreshTokenQuery { RefreshToken = input.RefreshToken});
    }
}