using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Entities;
using IdentityService.ViewModels;

namespace IdentityService.Services;

public interface IUserService
{
    Task<User> GetUserAsync(Guid id);
    Task CreateRoleAsync(AddUserRolesInput input);
    Task CreateAsync(User user, string password);
    void HasPassword(User user, string password);
    Task<bool> HasPasswordAsync(User user);
    Task<IList<string>> GetUserRolesNamesAsync(User user);
    Task<User> GetUserByUserNameAsync(string userName);
}