﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events;
using Identity.Api.Infrastructure.Exceptions;
using MassTransit;
using MediatR;
using IMediator = MediatR.IMediator;

namespace Identity.Api.Infrastructure.Brokers;

public class EventBus : IEventBus
{
    private readonly IPublishEndpoint publishEndpoint;
    private readonly IMediator mediator;
    
    public EventBus(IPublishEndpoint publishEndpoint, IMediator mediator)
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

    public Task Publish<TIntegrationEvent>(TIntegrationEvent @event) where TIntegrationEvent : IntegrationEvent
    {
        if (@event is null)
            throw new IdentityException.IdentityInternalException(
                new AppMessage($"notification:{nameof(@event)} is null."));

        return publishEndpoint.Publish(@event);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = new CancellationToken())
        where TRequest : IRequest
    {
        throw new NotImplementedException();
    }
}