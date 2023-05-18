using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using IdentityService.Entities;
using IdentityService.Exceptions;
using IdentityService.Options;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
    
    public Task<IList<string>> GetUserRolesNamesAsync(User user)
    {
        return userManager.GetRolesAsync(user);
    }
}

public class JwtTokenService : ITokenService
{
    private readonly IUserService userService;
    private readonly AppOptions.Jwt jwtOptions;

    public JwtTokenService(
        IUserService userService,
        AppOptions.Jwt jwtOptions
        )
    {
        this.userService = userService;
        this.jwtOptions = jwtOptions;
    }

    public async Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user)
    {
        var roles = await userService.GetUserRolesNamesAsync(user);
        var roleClaims = roles.Select(item => new Claim("roles", item));

        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString())
            }
            // .Union(userClaims)
            .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.DurationInMinutes),
            signingCredentials: signingCredentials);
        
        return new AuthViewModel.GetTokenOutput
            { AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) };
    }
}