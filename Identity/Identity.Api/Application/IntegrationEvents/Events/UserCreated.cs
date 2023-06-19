using EventBus.Events;

namespace Identity.Api.Application.IntegrationEvents.Events;

public record TestIntegrationEvent(string userName) : IntegrationEvent;
