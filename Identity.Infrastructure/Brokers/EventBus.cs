using EventBus.Abstractions;
using Identity.Infrastructure.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;
using IMediator = MediatR.IMediator;

namespace Identity.Infrastructure.Brokers;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public int Port { get; set; } = 5672;
}

public class EventBus : IEventBus
{
    // private readonly IPublishEndpoint publishEndpoint;
    private readonly IMediator mediator;
    // private readonly ISendEndpointProvider  sendEndpoint;

    public EventBus(IMediator mediator, IOptions<RabbitMqOptions> options)
    {
        // this.publishEndpoint = publishEndpoint;
        this.mediator = mediator;
        // this.sendEndpoint = sendEndpoint;
        
        var factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
            VirtualHost = options.Value.VirtualHost,
            Port = options.Value.Port,
            DispatchConsumersAsync = true
        };
    }
    
    public Task<TResponse> SendMediator<TResponse>(IRequest<TResponse> command)
    {
        return mediator.Send(command);
    }

    public Task PublishMediator<TNotification>(TNotification notification)
    {
        return mediator.Publish(notification);
    }

    public Task PublishIntegrated<TIntegrationEvent>(TIntegrationEvent @event) 
    {
        if (@event is null)
            throw new Exceptions.ApplicationException.Internal(
                new AppMessage($"notification:{nameof(@event)} is null."));
        
        // return publishEndpoint.Publish(@event);
        
        return Task.CompletedTask;
    }

    public async Task SendIntegrated<TRequest>(TRequest request, CancellationToken cancellationToken = new CancellationToken())
        where TRequest : IRequest
    {
        // await sendEndpoint.Send(request, cancellationToken);
    }
}