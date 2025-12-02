using System.Threading.Tasks;
using EventBus.MtuBus.Tests;
using Events;
using Identity.Api.Application.Commands.Users;
using Identity.Infrastructure.MtuBus;
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
    
    [HttpPost("int/event")]
    public async Task LoginAsync([FromBody] AddUserCommand command)
    {
        await _domainEventDispatcher.SendAsync(command);
    }

    [HttpGet("domain/event")]
    public async Task DomainLoginAsync()
    {
        await _domainEventDispatcher.PublishAsync(new TestDomainEvent(userName: "test domain event"));
    }
    
    
    [HttpGet("domain/request")]
    public async Task DomainRequestAsync()
    {
        // await _domainEventDispatcher.SendAsync(new LoginCommand("test domain event", "password", true));
    }
}