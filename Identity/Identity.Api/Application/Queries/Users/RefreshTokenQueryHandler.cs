using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Infrastructure.Brokers;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.SeedWork;
using MediatR;

namespace Identity.Api.Application.Queries.Users;

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, AuthViewModel.GetTokenOutput>
{
    private readonly IUserStore userStore;
    private readonly IEventBus eventHandler;

    public RefreshTokenQueryHandler(
        IUserStore userStore, IEventBus eventHandler)
    {
        this.userStore = userStore;
        this.eventHandler = eventHandler;
    }

    public async Task<AuthViewModel.GetTokenOutput> Handle(RefreshTokenQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userStore.GetTokenByRefreshAsync(request.RefreshToken);

        return await eventHandler.SendMediator(new LoginCommand(user.UserName, user.Password));
    }
}