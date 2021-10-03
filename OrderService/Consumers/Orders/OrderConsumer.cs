using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Models;

namespace OrderService.Consumers.Orders
{
    public class OrderConsumer : IConsumer<PaymentCreatedModel>
    {
        private readonly ILogger<OrderConsumer> logger;
        private readonly ISendEndpointProvider sendEndpointProvide;
        private readonly IPublishEndpoint publishEndpoint;

        public OrderConsumer(
            ILogger<OrderConsumer> logger,
            ISendEndpointProvider sendEndpointProvide,
            IPublishEndpoint publishEndpoint
        )
        {
            this.logger = logger;
            this.sendEndpointProvide = sendEndpointProvide;
            this.publishEndpoint = publishEndpoint;
        }
        public Task Consume(ConsumeContext<PaymentCreatedModel> context)
        {
            Console.WriteLine("orderservice --> get PaymentCreated");
            return Task.CompletedTask;
        }
    }
}   