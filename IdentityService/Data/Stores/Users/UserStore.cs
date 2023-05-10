using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data.Stores.Users
{
    public class UserStore : IUserStore
    {
        private readonly ThisDbContext identityDbContext;

        public UserStore(
            ThisDbContext identityDbContext
        )
        {
            this.identityDbContext = identityDbContext;
        }

        public void AddUser(User user)
        {
            identityDbContext.Users.Add(user);
        }

        public void DeleteUser(User user)
        {
            identityDbContext.Users.Remove(user);
        }

        public Task<User> GetUserAsync(Guid id)
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