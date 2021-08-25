using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OrderPaymentService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;
        private readonly IBus bus;

        public OrderController(ILogger<OrderController> logger
        ,IBus bus
        )
        {
            _logger = logger;
            this.bus = bus;
        }

        [HttpGet]
        public Task CreateOrderAsync()
        {
            var rng = new Random();
            bus.Publish<>

            return Task.CompletedTask;
        }
    }
}
