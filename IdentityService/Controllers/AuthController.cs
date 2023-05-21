using System.Threading.Tasks;
using AutoMapper;
using IdentityService.Consts;
using IdentityService.Entities;
using IdentityService.Exceptions;
using IdentityService.Services;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityService.Controllers;

[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ITokenGeneratorService tokenGenerator;
    private readonly ITokenService tokenService;
    private readonly IMapper mapper;
    private readonly IMemoryCache cache;

    public AuthController(
        IUserService userService,
        ITokenGeneratorService tokenGenerator,
        ITokenService tokenService,
        IMapper mapper,
        IMemoryCache cache
    )
    {
        this.userService = userService;
        this.tokenGenerator = tokenGenerator;
        this.tokenService = tokenService;
        this.mapper = mapper;
        this.cache = cache;
    }
    
    [HttpPost("login")]
    public async Task<AuthViewModel.GetTokenOutput> LoginAsync(AuthViewModel.LoginInput input)
    {
        var user = await userService.GetUserByUserNameAsync(input.UserName);
        if (user is null)
            throw new IdentityException.IdentityInternalException(AppMessages.UserNotFound);

        if (!await userService.HasPasswordAsync(user))
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
        
        cache.Set(CacheKeys.Token + token.Reference, token);
    }

    [HttpPost("refresh")]
    public async Task<AuthViewModel.GetTokenOutput> RefreshTokenAsync(AuthViewModel.RefreshTokenInput input)
    {
        var token = await tokenService.GetTokenByRefreshAsync(input.RefreshToken);
        if (token is null)
            throw new IdentityException.IdentityInternalException(AppMessages.UserNotFound);

        var user = await userService.GetUserAsync(token.UserId);
        if (user is null)
            throw new IdentityException.IdentityInternalException(AppMessages.UserNotFound);
        
        return await GetTokenAsync(user);
    }
}