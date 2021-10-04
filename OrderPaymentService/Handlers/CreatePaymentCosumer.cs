using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Models;
using OrderPaymentService.Models;

namespace OrderPaymentService.Handlers
{
    public class CreatePaymentCosumer : IConsumer<CreateOrderPaymentModel>
    {
        private readonly ILogger<CreatePaymentCosumer> logger;
        private readonly ISendEndpointProvider sendEndpointProvide;
        private readonly IPublishEndpoint publishEndpoint;

        public CreatePaymentCosumer(
            ILogger<CreatePaymentCosumer> logger,
            ISendEndpointProvider sendEndpointProvide,
            IPublishEndpoint publishEndpoint
        )
        {
            this.logger = logger;
            this.sendEndpointProvide = sendEndpointProvide;
            this.publishEndpoint = publishEndpoint;
        }

        public Task Consume(ConsumeContext<CreateOrderPaymentModel> context)
        {
            //TODO
            logger.LogInformation("payment created!");

            // bus.Send(new PaymentCreated { OrderId = context.Message.OrderId, Created = true });
            //  sendEndpointProvide.Send(new PaymentCreated { OrderId = context.Message.OrderId, Created = true });
             publishEndpoint.Publish<PaymentCreatedModel>(new  { OrderId = context.Message.OrderId, Created = true });
            return Task.CompletedTask;
        }
    }
}