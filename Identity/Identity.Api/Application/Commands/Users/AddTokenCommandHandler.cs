using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Aggregates.Users;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

public class AddTokenCommandHandler : IRequest<AddTokenCommand>
{
    private readonly IUserStore userStore;

    public AddTokenCommandHandler(
        IUserStore userStore
    )
    {
        this.userStore = userStore;
    }
    public async Task Handle(AddTokenCommand request, CancellationToken cancellationToken)
    {
        request.User.AddTokens(request.AccessToken, request.RefreshToken, request.ExpireDate);
    }
}