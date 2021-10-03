using System.Reflection.Emit;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Data.Stores.OrderProducts
{
    public class OrderProductStore : IOrderProductStore
    {
        private readonly OrderDbContext context;

        public OrderProductStore(
            OrderDbContext context
        )
        {
            this.context = context;
        }

        public Task<List<OrderProduct>> GetAllOrderProductsAsync()
        {
            return context.OrderProducts.ToListAsync();
        }

        public Task<OrderProduct> GetOrderProductAsync(int id)
        {
            return context.OrderProducts.FirstOrDefaultAsync(item => item.Id == id);
        }

        public Task AddOrderProductsAsync(OrderProduct orderProduct)
        {
            return context.OrderProducts.AddAsync(orderProduct).AsTask();
        }
    }
}