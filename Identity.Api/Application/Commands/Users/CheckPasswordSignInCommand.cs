using System;
using Identity.Api.ViewModels;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

public class CheckPasswordSignInCommand : IRequest<bool>
{
    public CheckPasswordSignInCommand(string userName, string password, bool doHashPassword)
    {
        Password = password;
        DoHashPassword = doHashPassword;
        UserName = userName;
    }
    public string UserName { get; }
    public string Password { get; }
    public bool DoHashPassword { get; }
}