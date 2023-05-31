using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Entities;
namespace IdentityService.Data.Stores.Users
{
    public interface IUserStore
    {
         Task<List<User>> GetUsersAsync();
         Task<User> GetUserAsync(Guid id);
         void AddUser(User user);
         void DeleteUser(User user);
         void UpdateUser(User user);
        Task SaveChangeAsync();
    }
}