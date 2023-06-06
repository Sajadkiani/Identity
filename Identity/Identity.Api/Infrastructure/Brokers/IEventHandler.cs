using System;
using System.Threading.Tasks;
using MediatR;

namespace Identity.Api.Infrastructure.Brokers;

public interface IEventHandler
{
    Task<TResponse> SendMediator<TResponse>(IRequest<TResponse> command);
    Task PublishMediator<TNotification>(TNotification notification);
}