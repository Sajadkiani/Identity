using Identity.Api.ViewModels;
using MediatR;

namespace Identity.Api.Application.Queries.Users;

public class RefreshTokenQuery : IRequest<AuthViewModel.GetTokenOutput>
{
    public string RefreshToken { get; set; }
}