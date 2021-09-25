using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Entities;
namespace IdentityService.Data.Stores.Users
{
    public interface IUserStore
    {
         Task<List<User>> GetUsersAsync();
         Task<User> GetUserAsync(int id);
         Task AddUserAsync(User user);
         void DeleteUser(User user);
         void UpdateUser(User user);
    }
}