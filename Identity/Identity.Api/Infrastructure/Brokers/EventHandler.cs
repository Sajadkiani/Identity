using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Api.Infrastructure.Exceptions;
using MassTransit;
using MediatR;

namespace Identity.Api.Infrastructure.Brokers;

public class EventHandler : IEventHandler
{
    private readonly IPublishEndpoint publishEndpoint;
    private readonly IMediator mediator;
    
    public EventHandler(IPublishEndpoint publishEndpoint, IMediator mediator)
    {
        this.publishEndpoint = publishEndpoint;
        this.mediator = mediator;
    }
    
    public Task<TResponse> SendMediator<TResponse>(IRequest<TResponse> command)
    {
        return mediator.Send(command);
    }
    
    public Task PublishMediator<TNotification>(TNotification notification)
    {
        return mediator.Publish(notification);
    }

    public Task Publish<TNotification>(TNotification notification)
    {
        if (notification is null)
            throw new IdentityException.IdentityInternalException(
                new AppMessage($"notification:{nameof(notification)} is null."));
        
        return publishEndpoint.Publish(notification);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = new CancellationToken()) where TRequest : IRequest
    {
        throw new NotImplementedException();
    }
}