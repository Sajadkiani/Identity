using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data.Stores.Users
{
    public class UserStore : IUserStore
    {
        private readonly IdentityDbContext identityDbContext;

        public UserStore(
            IdentityDbContext identityDbContext
        )
        {
            this.identityDbContext = identityDbContext;
        }
        public Task AddUserAsync(User user)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetUserAsync(int id)
        {
            return identityDbContext.Users.FirstOrDefaultAsync(item => item.Id == id);
        }

        public Task<List<User>> GetUsersAsync()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateUser(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}