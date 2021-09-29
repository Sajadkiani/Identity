using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransit;
using Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> logger;
        private readonly IPublishEndpoint publishEndpoint;

        public OrderController(
         ILogger<OrderController> logger,
         IPublishEndpoint publishEndpoint
        )
        {
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task CreateOrderAsync()
        {
            var orderId = Guid.NewGuid();
            logger.LogInformation("--> add order");
            var x = publishEndpoint.Publish(new CreateOrderPaymentModel{ OrderId = 1 });
            //var x = await client.GetResponse<OrderCreated>(new CreateOrderCommand { OrderId = orderId });
        }
    }
}
