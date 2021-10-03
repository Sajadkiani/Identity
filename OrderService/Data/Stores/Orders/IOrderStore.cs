using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Entities;

namespace OrderService.Data.Stores.Orders
{
    public interface IOrderStore
    {
        Task AddOrderAsync(Order order);
        Task<List<Order>> GetAllOrderAsync();
        Task<Order> GetOrderAsync(int id);
        Task SaveChangeAsync();
    }
}