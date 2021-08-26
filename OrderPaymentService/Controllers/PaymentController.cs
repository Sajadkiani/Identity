using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Commands.Order.Payments;
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

        public PaymentController(ILogger<PaymentController> logger
        ,IBus bus
        )
        {
            _logger = logger;
            this.bus = bus;
        }

        [HttpGet]
        public Task CreateOrderAsync()
        {
            var orderId = Guid.NewGuid();
            _logger.LogInformation("Add order");
            bus.Publish(new CreatePaymentCommand{OrderId=orderId});

            return Task.CompletedTask;
        }
    }
}
