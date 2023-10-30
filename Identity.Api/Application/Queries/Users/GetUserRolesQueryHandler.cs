using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using MediatR;

namespace Identity.Api.Application.Queries.Users;

public class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, AuthViewModel.GetUserByUserNameOutput>
{
    private readonly IUserStore store;
    private readonly IMapper mapper;

    public GetUserByUserNameQueryHandler(
        IUserStore store,
        IMapper mapper)
    {
        this.store = store;
        this.mapper = mapper;
    }

    public async Task<AuthViewModel.GetUserByUserNameOutput> Handle(GetUserByUserNameQuery request,
        CancellationToken cancellationToken)
    {
        return mapper.Map<AuthViewModel.GetUserByUserNameOutput>(await store.GetByUserNameAsync(request.UserName));
    }
}

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
            $"select r.id, r.name from userroles as ur left join roles as r on ur.roleid = r.id where ur.userid ={request.UserId}");
        
    }
}