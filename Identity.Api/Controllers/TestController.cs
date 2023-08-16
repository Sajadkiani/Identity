using System.Collections.Generic;
using System.Threading.Tasks;
using Events;
using Identity.Domain.Events.Users;
using Identity.Infrastructure.MtuBus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IIntegrationEventDispatcher _integrationEventDispatcher;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public TestController(
        IIntegrationEventDispatcher integrationEventDispatcher,
        IDomainEventDispatcher domainEventDispatcher
    )
    {
        _integrationEventDispatcher = integrationEventDispatcher;
        _domainEventDispatcher = domainEventDispatcher;
    }
    
    [HttpGet("int/event")]
    public async Task LoginAsync()
    {
        await _integrationEventDispatcher.PublishAsync(nameof(TestIntegrationEvent), new TestIntegrationEvent
        {
            UserName = "integrated event"
        });
    }


    [HttpGet("domain/event")]
    public async Task DomainLoginAsync()
    {
        await _domainEventDispatcher.DispatchAsync(new List<INotification>
        {
            new TestDomainEvent("domain event")
        });
    }
}