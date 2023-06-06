using Identity.Domain.Aggregates.Users;
using Identity.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.EF.Stores;

public class UserStore : Repository<User, int>, IUserStore
{
    private readonly AppDbContext context;

    public IUnitOfWork UnitOfWork => context;

    public UserStore(AppDbContext context) : base(context)
    {
    }
    
    public Task<User> GetByUserNameAsync(string userName)
    {
        return context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public Task<User> GetTokenByRefreshAsync(string refreshToken)
    {
        return context.Users.Include(u => u.Tokens.Where(item => item.RefreshToken == refreshToken))
            .FirstOrDefaultAsync();
    }
}