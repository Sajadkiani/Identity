using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Entities;
using IdentityService.Exceptions;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> userManager;

    public UserService(
        UserManager<User> userManager
    )
    {
        this.userManager = userManager;
    }

    public Task<bool> HasPasswordAsync(User user)
    {
        return userManager.HasPasswordAsync(user);
    }
    
    public Task<User> GetUserByUserNameAsync(string userName)
    {
        return userManager.Users.FirstOrDefaultAsync(item => item.UserName == userName);
    }
    
    public Task<User> GetUserAsync(Guid id)
    {
        return userManager.Users.FirstOrDefaultAsync(item => item.Id == id);
    }
    
    public Task<IList<string>> GetUserRolesNamesAsync(User user)
    {
        return userManager.GetRolesAsync(user);
    }
    
    public void HasPassword(User user, string password)
    {
        userManager.PasswordHasher.HashPassword(user, password);
    }

    public async Task CreateAsync(User user, string password)
    {
        await CreateUserAsync(user, password,"Applicant");
    }

    private async Task CreateUserAsync(User user, string password, string roleName)
    {
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, roleName);
    }

    public async Task CreateRoleAsync(AddUserRolesInput input)
    {
        var user = await userManager.FindByIdAsync(input.UserId.ToString());
        if (user is null)
            throw new IdentityException.IdentityInternalException(AppMessages.UserNotFound);
        
        await userManager.AddToRoleAsync(user, input.RoleName);
    }
}