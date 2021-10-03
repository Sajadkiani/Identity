using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderService.Data.Stores.Orders;
using OrderService.Entities;
using OrderService.Models;
using OrderService.ViewModels;

namespace OrderService.Controllers
{
    [Route("api/orderproducts")]
    public class OrderProductController :ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOrderStore orderStore;

        public OrderProductController(
            IMapper mapper,
            IOrderStore orderStore
        )
        {
            this.mapper = mapper;
            this.orderStore = orderStore;
        }

        [HttpGet("{userId}")]
        public async Task<GetOrderModel> GetUserAsync(int userId)
        {
            var order = await orderStore.GetOrderAsync(userId);
            return new GetOrderModel
            {
                Id = order.Id,
                CreateOn = order.CreateOn,
                OrderNo = order.OrderNo
            };
        }

        [HttpPost]
        public async Task AddUserAsync(GetOrderVm vm)
        {
            var order = mapper.Map<Order>(vm);
            await orderStore.AddOrderAsync(order);
            await orderStore.SaveChangeAsync();
        }

        [HttpGet("all")]
        public async Task<List<GetOrderModel>> GetAllUserAsync()
        {
            var orders = await orderStore.GetAllOrderAsync();
            return mapper.Map<List<GetOrderModel>>(orders);
        }
    }
}