using MediatR;

namespace Identity.Infrastructure.MtuBus;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<INotification> domainEvents);
}