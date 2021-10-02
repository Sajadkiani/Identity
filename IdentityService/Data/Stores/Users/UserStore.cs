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
            return identityDbContext.Users.AddAsync(user).AsTask();
        }

        public void DeleteUser(User user)
        {
            identityDbContext.Users.Remove(user);
        }

        public Task<User> GetUserAsync(int id)
        {
            return identityDbContext.Users.FirstOrDefaultAsync(item => item.Id == id);
        }

        public Task<List<User>> GetUsersAsync()
        {
           return identityDbContext.Users.ToListAsync();
        }

        public void UpdateUser(User user)
        {
            identityDbContext.Users.Update(user);
        }

        public Task SaveChangeAsync() => identityDbContext.SaveChangesAsync(); 
    }
}