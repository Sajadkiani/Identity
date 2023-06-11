using System;
using Identity.Domain.Aggregates.Users;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

public class AddTokenCommand : IRequest<int>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireDate { get; set; }
    
    public User User { get; set; }
    public int UserId { get; set; }
}