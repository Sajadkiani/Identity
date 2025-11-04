using Events;
using Identity.Domain.Events.Users;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.MtuBus.Consumers;

public class TestConsumer : MTUConsumer<TestIntegrationEvent>
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private const string QueueName = "test-queue-name";
    public TestConsumer(
        IMtuBusConnectionManager connectionManager,
        ILogger<TestConsumer> logger, IDomainEventDispatcher eventDispatcher) :
        base(QueueName, connectionManager, logger)
    {
        _eventDispatcher = eventDispatcher;
    }
    
    protected override async Task ConsumeAsync(TestIntegrationEvent message)
    {
        await _eventDispatcher.DispatchAsync(new TestDomainEvent(message.UserName));
    }
}