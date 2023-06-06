using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Aggregates.Users;
using IdentityService.Api.Application.Commands.Users;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

public class AddUserCommandHandler : INotificationHandler<AddUserCommand>
{
    public Task Handle(AddUserCommand notification, CancellationToken cancellationToken)
    {
        var user = new User(notification.Name, notification.Family, notification.UserName, notification.Email,
            notification.Password, notification.Gender);
        
    }
}