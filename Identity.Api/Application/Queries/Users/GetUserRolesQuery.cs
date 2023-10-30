using System.Collections.Generic;
using Identity.Api.ViewModels;
using MediatR;

namespace Identity.Api.Application.Queries.Users;

public class GetUserByUserNameQuery : IRequest<AuthViewModel.GetUserByUserNameOutput>
{
    public string UserName { get; set; }

    public GetUserByUserNameQuery(string userName)
    {
        UserName = userName;
    }

}

public class GetUserRolesQuery : IRequest<IEnumerable<AuthViewModel.UserRoleOutput>>
{
    public GetUserRolesQuery(int userId)
    {
        UserId = userId;
    }

    public int UserId { get; }
}