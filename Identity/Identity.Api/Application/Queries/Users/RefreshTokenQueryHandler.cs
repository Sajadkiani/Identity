using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Infrastructure.Brokers;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.SeedWork;
using MediatR;

namespace Identity.Api.Application.Queries.Users;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IEnumerable<AuthViewModel.UserRoleOutput>>
{
    private readonly IQueryExecutor queryExecutor;

    public GetUserRolesQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        this.queryExecutor = queryExecutor;
    }
    
    public Task<IEnumerable<AuthViewModel.UserRoleOutput>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        return queryExecutor.QueryAsync<AuthViewModel.UserRoleOutput>(
            $"select r.id, r.name from userroles as ur left join roles as r on ur.roleid = r.id where ur.useid ={request.UserId}");
        
    }
}

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, AuthViewModel.GetTokenOutput>
{
    private readonly IUserStore userStore;
    private readonly IEventHandler eventHandler;

    public RefreshTokenQueryHandler(
        IUserStore userStore, IEventHandler eventHandler)
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