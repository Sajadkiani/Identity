using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransit;
using OrderService.Constants;
using OrderService.Models;
using Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> logger;
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IBus bus;

        public OrderController(
         ILogger<OrderController> logger,
         ISendEndpointProvider sendEndpointProvider, 
         IBus bus
        )
        {
            this.logger = logger;
            this.sendEndpointProvider = sendEndpointProvider;
            this.bus = bus;
        }

        [HttpGet]
        public async Task CreateOrderAsync()
        {
            var orderId = new Random().Next();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://localhost/{QueueNames.create_order_payment}"));

            await endpoint.Send<CreateOrderPaymentModel>(new { OrderId = orderId });
         
            logger.LogInformation("--> add order");
            // var x = publishEndpoint.Publish(new CreateOrderPaymentModel{ OrderId = 1 });
            //var x = await client.GetResponse<OrderCreated>(new CreateOrderCommand { OrderId = orderId });
        }
    }
}
