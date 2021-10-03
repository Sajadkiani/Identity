using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OrderPaymentService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {

        private readonly ILogger<PaymentController> _logger;
        private readonly IBus bus;
        private readonly ISendEndpointProvider sendEndpointProvider;

        public PaymentController(ILogger<PaymentController> logger
        ,IBus bus
        ,ISendEndpointProvider sendEndpointProvider
        )
        {
            _logger = logger;
            this.bus = bus;
            this.sendEndpointProvider = sendEndpointProvider;
        }

        [HttpGet]
        public Task CreateOrderAsync()
        {
            var orderId = Guid.NewGuid();
            _logger.LogInformation("Add order");
            // bus.Publish(new CreatePaymentCommand{OrderId=orderId});

            return Task.CompletedTask;
        }
    }
}
