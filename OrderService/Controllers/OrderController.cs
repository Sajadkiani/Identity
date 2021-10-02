using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransit;
using Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> logger;
        private readonly IBus bus;

        // private readonly IPublishEndpoint publishEndpoint;

        public OrderController(
         ILogger<OrderController> logger,
         IBus bus 
        //  IPublishEndpoint publishEndpoint
        )
        {
            this.logger = logger;
            this.bus = bus;
            // this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task CreateOrderAsync()
        {
            var sendEndpoint = await bus.GetSendEndpoint(new Uri(ConfigurationManager.AppSettings["MyCommandQueueFullUri"]));
            await sendEndpoint.Send<MyCommand>(...);
            var orderId = Guid.NewGuid();
            logger.LogInformation("--> add order");
            var x = bus.Send(new CreateOrderPaymentModel{ OrderId = 1 });
            //var x = await client.GetResponse<OrderCreated>(new CreateOrderCommand { OrderId = orderId });
        }
    }
}
