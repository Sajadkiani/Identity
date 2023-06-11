using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Events.Users;
using MediatR;

namespace Identity.Api.Application.DomainEventHandlers.Users;

public class SampleDomainEventHandler : INotificationHandler<SampleDomainEvent>
{
    public Task Handle(SampleDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}