using EventBus.Events;

namespace Events;

// public record TestIntegrationEvent: IntegrationEvent
public record TestIntegrationEvent
{
    public string UserName { get; init; }
}
