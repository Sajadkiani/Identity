using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityService.Data;
using IdentityService.Entities;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class TokenService : ITokenService
{
    private readonly IMapper mapper;
    private readonly ThisDbContext context;
    private readonly UserManager<User> userManager;

    public TokenService(
        IMapper mapper,
        ThisDbContext context,
        UserManager<User> userManager)
    {
        this.mapper = mapper;
        this.context = context;
        this.userManager = userManager;
    }

    public async Task AddTokenAsync(AuthViewModel.AddTokenInput input)
    {
        var token = mapper.Map<Token>(input);
        
        
        await context.Tokens.AddAsync(token);
        await context.SaveChangesAsync();
    }
    
    public Task<Token> GetTokenByRefreshAsync(string refresh)
    {
        return context.Tokens.Where(item => item.RefreshToken == refresh).FirstOrDefaultAsync();
    }
}

public class RoleService : IRoleService
{
    private readonly RoleManager<Role> roleManager;
    private readonly IMapper mapper;

    public RoleService(
        RoleManager<Role> roleManager
        , IMapper mapper
    )
    {
        this.roleManager = roleManager;
        this.mapper = mapper;
    }

    public async Task AddRoleAsync(AddRoleInput input)
    {
        var role = mapper.Map<Role>(input);
        await roleManager.CreateAsync(role);
    }
}