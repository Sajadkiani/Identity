using System.Threading;
using System.Threading.Tasks;
using Identity.Api.Infrastructure.AppServices;
using Identity.Api.Infrastructure.Exceptions;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.Aggregates.Users.Enums;
using IdentityService.Options;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Api.Application.Commands.Users;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthViewModel.GetTokenOutput>
{
    private readonly IUserStore userStore;
    private readonly IPasswordService passwordService;
    private readonly ITokenGeneratorService tokenService;
    private readonly IMemoryCache cache;
    private readonly AppOptions.Jwt jwt;

    public LoginCommandHandler(
        IUserStore userStore,
        IPasswordService passwordService,
        ITokenGeneratorService tokenService,
        IMemoryCache cache,
        AppOptions.Jwt jwt
    )
    {
        this.userStore = userStore;
        this.passwordService = passwordService;
        this.tokenService = tokenService;
        this.cache = cache;
        this.jwt = jwt;
    }
    
    public async Task<AuthViewModel.GetTokenOutput> Handle(LoginCommand notification, CancellationToken cancellationToken)
    {
        var user = await userStore.GetByUserNameAsync(notification.UserName);
        if (user is null)
        {
            throw new IdentityException.IdentityNotFoundException(AppMessages.UserNotFound);
        }
        
        if (user.Status != UserStatus.Active)
        {
            throw new IdentityException.IdentityUnauthorizedException();
        }

        var incomePassword = passwordService.HashPassword(user.Id, notification.Password, notification.UserName);
        if (user.Password != incomePassword)
        {
            throw new IdentityException.IdentityUnauthorizedException();
        }

        var token = await tokenService.GenerateTokenAsync(user);
        
        user.AddTokens(token.AccessToken, token.RefreshToken, token.ExpireDate);
        
        //TODO: check for add token in database

        return token;
    }
}