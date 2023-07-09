using System;
using Identity.Api.ViewModels;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

public class LoginCommand : IRequest<AuthViewModel.GetTokenOutput>
{
    public LoginCommand(string userName, string password)
    {
        Password = password;
        UserName = userName;
    }
    public string UserName { get; }
    public string Password { get; }
}