using MassTransit;

namespace Common.Services.Brokers;

public class MasstransitBroker : IBroker
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MasstransitBroker(
        IPublishEndpoint publishEndpoint
        )
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<TEvent>(TEvent @event)
    {
        if (@event is null) throw new Exception($"event:{nameof(@event)} is null.");
        return _publishEndpoint.Publish(@event);
    }
}