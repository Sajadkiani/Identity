using System.Threading.Tasks;
using IdentityService.Exceptions;
using IdentityService.Services;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ITokenService tokenService;

    public AuthController(
        IUserService userService,
        ITokenService tokenService
        )
    {
        this.userService = userService;
        this.tokenService = tokenService;
    }
    
    [HttpPost("login")]
    public async Task<AuthViewModel.LoginOutput> LoginAsync(AuthViewModel.LoginInput input)
    {
        var user = await userService.GetUserByUserNameAsync(input.UserName);
        if (user is null)
            throw new IdentityException.IdentityInternalException(AppMessages.UserNotFound);

        if (!await userService.HasPasswordAsync(user))
            throw new IdentityException.IdentityInternalException(AppMessages.UserNotFound);

        var token = await tokenService.GenerateTokenAsync(user);
        return new AuthViewModel.LoginOutput()
        {
            AccessToken = token.AccessToken
        };
    }
        
    public async Task<AuthViewModel.LoginOutput> RefreshTokenAsync(AuthViewModel.RefreshTokenInput input)
    {

        return new AuthViewModel.LoginOutput();
    }
}