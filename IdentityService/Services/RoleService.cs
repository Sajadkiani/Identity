using System;
using System.Threading.Tasks;
using AutoMapper;
using IdentityService.Entities;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

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