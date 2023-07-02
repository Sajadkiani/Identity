using IntegrationEventLogEF.Services;

namespace Events;

// public record TestIntegrationEvent: IntegrationEvent
public class TestIntegrationEvent : IntegrationEvent
{
    public string UserName { get; init; }
}
    