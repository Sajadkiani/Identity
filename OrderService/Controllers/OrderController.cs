using System;
using System.Threading.Tasks;
using Common.Commands.Order;
using Common.Commands.Order.Payments;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> logger;
        private readonly IRequestClient<CreateOrderCommand> client;

        public OrderController(
         ILogger<OrderController> logger,
         IRequestClient<CreateOrderCommand> client
        )
        {
            this.logger = logger;
            this.client = client;
        }

        [HttpGet]
        public async Task CreateOrderAsync()
        {
            var orderId = Guid.NewGuid();
            logger.LogInformation("Add order");
            var x = await client.GetResponse<OrderCreated>(new CreateOrderCommand { OrderId = orderId });
        }
    }
}
