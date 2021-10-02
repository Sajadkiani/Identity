using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Data.Stores.Orders
{
    public class OrderStore : IOrderStore
    {
        private readonly OrderDbContext context;

        public OrderStore(
            OrderDbContext context
        )
        {
            this.context = context;
        }

        public Task<List<Order>> GetAllOrderAsync()
        {
            return context.Orders.ToListAsync();
        }

        public Task<Order> GetOrderAsync(int id)
        {
            return context.Orders.FirstOrDefaultAsync(item => item.Id == id);
        }

        public Task AddOrderAsync(Order order)
        {
            return context.Orders.AddAsync(order).AsTask();
        }

        public Task SaveChangeAsync()=> context.SaveChangesAsync();
    }
}