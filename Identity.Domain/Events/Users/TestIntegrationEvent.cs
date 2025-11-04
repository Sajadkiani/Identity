using AppEvents;
using IntegrationEventLogEF.Services;

namespace Events;

public class TestIntegrationEvent : IntegrationEvent
{
    public string UserName { get; init; }
}
    