using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Entities;

namespace IdentityService.Services;

public interface IUserService
{
    Task<bool> HasPasswordAsync(User user);
    Task<IList<string>> GetUserRolesNamesAsync(User user);
    Task<User> GetUserByUserNameAsync(string userName);
}