using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransit;
using Models;
using OrderService.Constants;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> logger;
        private readonly ISendEndpointProvider sendEndpointProvider;
        // private readonly IPublishEndpointProvider publishEndpointProvider;
        private readonly IBus bus;

        public OrderController(
         ILogger<OrderController> logger,
         ISendEndpointProvider sendEndpointProvider, 
        //  IPublishEndpointProvider publishEndpointProvider, 
         IBus bus
        )
        {
            this.logger = logger;
            this.sendEndpointProvider = sendEndpointProvider;
            // this.publishEndpointProvider = publishEndpointProvider;
            this.bus = bus;
        }

        [HttpGet("add")]
        public async Task CreateOrderAsync()
        {
            var orderId = new Random().Next();
            var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{QueueNames.createPayment}"));
            await endpoint.Send(new CreatePayment{ OrderId = orderId});
            await bus.Publish(new CreatePayment{ OrderId = orderId});
            logger.LogInformation("--> add order");
            // var x = publishEndpoint.Publish(new CreateOrderPaymentModel{ OrderId = 1 });
            //var x = await client.GetResponse<OrderCreated>(new CreateOrderCommand { OrderId = orderId });
        }
    }
}
