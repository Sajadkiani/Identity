using System;
using Identity.Api.ViewModels;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

public class LoginCommand : IRequest<AuthViewModel.GetTokenOutput>
{
    public LoginCommand(string userName, string password, bool doHashPassword)
    {
        Password = password;
        DoHashPassword = doHashPassword;
        UserName = userName;
    }
    public string UserName { get; }
    public string Password { get; }
    public bool DoHashPassword { get; }
}