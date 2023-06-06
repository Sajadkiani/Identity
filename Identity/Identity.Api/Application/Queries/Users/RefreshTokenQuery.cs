using System.Threading;
using System.Threading.Tasks;
using Identity.Api.Infrastructure.Brokers;
using Identity.Domain.Aggregates.Users;
using IdentityService.Api.Application.Commands.Users;
using IdentityService.ViewModels;
using MediatR;

namespace IdentityService.Api.Application.Queries.Users;

public class RefreshTokenQuery : IRequest<AuthViewModel.GetTokenOutput>
{
    public string RefreshToken { get; set; }
}

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, AuthViewModel.GetTokenOutput>
{
    private readonly IUserStore userStore;
    private IEventHandler eventHandler;

    public RefreshTokenQueryHandler(
        IUserStore userStore, IEventHandler eventHandler)
    {
        this.userStore = userStore;
        this.eventHandler = eventHandler;
    }
    
    public async Task<AuthViewModel.GetTokenOutput> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var user = await userStore.GetTokenByRefreshAsync(request.RefreshToken);
        var token = 
        return await eventHandler.SendMediator(new LoginCommand(user.Tokens.Fir))
    }
}