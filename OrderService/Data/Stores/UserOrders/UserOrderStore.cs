using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Data.Stores.UserOrders
{
    public class UserOrderStore : IUserOrderStore
    {
        private readonly OrderDbContext context;

        public UserOrderStore(
            OrderDbContext context
        )
        {
            this.context = context;
        }

         public Task<List<UserOrder>> GetAllUserOrderAsync()
        {
            return context.UserOrders.ToListAsync();
        }

        public Task<UserOrder> GetUserOrderAsync(int id)
        {
            return context.UserOrders.FirstOrDefaultAsync(item => item.Id == id);
        }

        public Task AddUserOrderAsync(UserOrder order)
        {
            return context.UserOrders.AddAsync(order).AsTask();
        }
    }
}