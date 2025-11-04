using MediatR;

namespace Identity.Infrastructure.MtuBus;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAsync(IEnumerable<INotification> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }

    public async Task DispatchAsync(INotification domainEvent)
    {
        await _mediator.Publish(domainEvent);
    }
}