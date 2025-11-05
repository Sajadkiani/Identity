using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Identity.Api.Application.Commands.Users;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using Identity.Infrastructure.Dapper;
using MediatR;

namespace Identity.Api.Application.Queries.Users;

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, AuthViewModel.GetTokenOutput>
{
    private readonly IUserStore userStore;
    private readonly IDomainEventDispatcher eventHandler;
    private readonly IQueryExecutor queryExecutor;

    public RefreshTokenQueryHandler(
        IUserStore userStore, 
        IDomainEventDispatcher eventHandler,
        IQueryExecutor queryExecutor 
        )
    {
        this.userStore = userStore;
        this.eventHandler = eventHandler;
        this.queryExecutor = queryExecutor;
    }

    public async Task<AuthViewModel.GetTokenOutput> Handle(RefreshTokenQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userStore.GetTokenByRefreshAsync(request.RefreshToken);
        if (user is null)
        {
            //unauthenticated err
        }

        //TODO: we can put this query string separate file or even make store for it
        //TODO: think about it's ok to use dapper here
        var token =
            (await queryExecutor.QueryAsync<Token>(
                $"select * from {nameof(Token)}s where userid == {user.Id} and {nameof(Token.ExpireDate)} > '{DateTime.Now}'"))
            .FirstOrDefault();

        if (token is null)
        {
            // 401 err
        }

        return await eventHandler.SendMediator(new LoginCommand(user.UserName, user.Password, doHashPassword: false ));
    }
}