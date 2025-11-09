using System.Threading;
using System.Threading.Tasks;
using EventBus.MtuBus.Tests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Api.Application.DomainEventHandlers.Users;

public class TestDomainEventNotificationHandler : INotificationHandler<TestDomainEvent>
{
    private readonly ILogger<TestDomainEventNotificationHandler> _logger;

    public TestDomainEventNotificationHandler(
        ILogger<TestDomainEventNotificationHandler> logger 
        )
    {
        _logger = logger;
    }
    
    public async Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(TestDomainEventNotificationHandler)} processed the notification {nameof(notification.GetType)}");
    }
}
