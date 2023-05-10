using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Services;
using Common.Services.Brokers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransit;
using Models;
using OrderService.Constants;
using OrderService.Data.Stores.Orders;
using OrderService.Entities;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> logger;
        private readonly IBroker _broker;
        private readonly ICurrentUser _currentUser;
        private readonly IOrderStore _orderStore;

        public OrderController(
         ILogger<OrderController> logger,
         IBroker broker,
         ICurrentUser currentUser,
         IOrderStore orderStore)
        {
            this.logger = logger;
            _broker = broker;
            _currentUser = currentUser;
            _orderStore = orderStore;
        }

        [HttpGet("add")]
        public async Task CreateOrderAsync()
        {
            var order = new Order();
            order.OrderNo = new Random().Next().ToString();
            order.UserId = _currentUser.UserId;
            await _orderStore.AddOrderAsync(order);
            await _orderStore.SaveChangeAsync();
            
            logger.LogInformation("--> add order");
        }
    }
}
