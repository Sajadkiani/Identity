using AppDomain.SeedWork;

namespace Identity.Domain.Aggregates.Users;

public interface IUserStore : IRepository<User, int>
{
    Task<User> GetByUserNameAsync(string userName);
    Task AddUserAsync(User user);
}