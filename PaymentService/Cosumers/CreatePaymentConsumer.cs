using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Models;

namespace PaymentService.Cosumers
{
    public class CreatePaymentConsumer : IConsumer<CreatePayment>
    {
        private readonly ILogger<CreatePaymentConsumer> logger;
        private readonly ISendEndpointProvider sendEndpointProvide;
        private readonly IPublishEndpoint publishEndpoint;

        public CreatePaymentConsumer(
            ILogger<CreatePaymentConsumer> logger,
            ISendEndpointProvider sendEndpointProvide,
            IPublishEndpoint publishEndpoint
        )
        {
            this.logger = logger;
            this.sendEndpointProvide = sendEndpointProvide;
            this.publishEndpoint = publishEndpoint;
        }   

        public Task Consume(ConsumeContext<CreatePayment> context)
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