using System.Threading;
using System.Threading.Tasks;
using Identity.Api.Infrastructure.AppServices;
using Identity.Api.Infrastructure.Exceptions;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.Aggregates.Users.Enums;
using IdentityService.Api.Application.Commands.Users;
using IdentityService.ViewModels;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthViewModel.GetTokenOutput>
{
    private readonly IUserStore userStore;
    private readonly IPasswordService passwordService;
    private readonly ITokenGeneratorService tokenService;

    public LoginCommandHandler(
        IUserStore userStore,
        IPasswordService passwordService,
        ITokenGeneratorService tokenService
    )
    {
        this.userStore = userStore;
        this.passwordService = passwordService;
        this.tokenService = tokenService;
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

        return await tokenService.GenerateTokenAsync(user);
    }
}