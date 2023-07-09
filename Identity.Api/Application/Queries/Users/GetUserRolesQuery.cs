using System.Collections.Generic;
using Identity.Api.ViewModels;
using MediatR;

namespace Identity.Api.Application.Queries.Users;

public class GetUserRolesQuery : IRequest<IEnumerable<AuthViewModel.UserRoleOutput>>
{
    public GetUserRolesQuery(int userId)
    {
        UserId = userId;
    }

    public int UserId { get; }
}