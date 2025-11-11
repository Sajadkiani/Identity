using Identity.Domain.Aggregates.Users;
using Identity.Infrastructure.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.ORM.EF.Stores;

public class UserStore : Repository<User, int>, IUserStore
{
    public UserStore(AppDbContext context) : base(context)
    {
    }
    
    public Task<User> GetByUserNameAsync(string userName)
    {
        return context.Users.FirstOrDefaultAsync(u => u.UserName == userName)!;
    }
    
    public Task AddUserAsync(User user)
    {
        return context.Users.AddAsync(user).AsTask();
    }

    public Task<List<Role>> GetUserIncludeRolesAsync(int userId)
    {
        return context.Users
            .Where(item => item.Id == userId)
            .SelectMany(item => item.UserRoles).Select(item => item.Role)
            .ToListAsync();
    }
}