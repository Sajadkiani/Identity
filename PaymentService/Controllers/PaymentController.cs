using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using OrderService.Constants;

namespace OrderPaymentService.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IBus bus;
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IPublishEndpoint publishEndpoint;

        public PaymentController(ILogger<PaymentController> logger
        , IBus bus
        , ISendEndpointProvider sendEndpointProvider
        , IPublishEndpoint publishEndpoint

        )
        {
            _logger = logger;
            this.bus = bus;
            this.sendEndpointProvider = sendEndpointProvider;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet("sendToOrder")]
        public async Task SendOrderAsync()
        {
            _logger.LogInformation("Add order");
             var orderId = new Random().Next();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://localhost/{QueueNames.payment_created}"));
            await endpoint.Send(new PaymentCreatedModel { OrderId = orderId });
            return;
        }

        [HttpGet("publishToOrder")]
        public async Task PublishOrderAsync()
        {
            _logger.LogInformation("Add order");
            var orderId = new Random().Next();

            await publishEndpoint.Publish(new PaymentCreatedModel{OrderId=orderId});
            //  await bus.Publish<PaymentCreatedModel>(new  { OrderId = orderId, Created = true });

            return;
        }
    }
}
