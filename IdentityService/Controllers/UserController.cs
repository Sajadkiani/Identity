using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using IdentityService.Data.Stores.Users;
using IdentityService.Entities;
using IdentityService.Models;
using IdentityService.Services;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IRoleService roleService;

        public UserController(
            IUserService userService,
            IMapper mapper,
            IRoleService roleService
        )
        {
            this.userService = userService;
            this.mapper = mapper;
            this.roleService = roleService;
        }

        [HttpPost]
        public async Task AddUserAsync([FromBody] AddUserInput input)
        {
            var user = mapper.Map<User>(input);
            await userService.CreateAsync(user, input.Password);
        }
        
        [HttpPost("roles")]
        public async Task AddRoleAsync([FromBody] AddRoleInput input)
        {
            await roleService.AddRoleAsync(input);
        }

        [HttpPost("userRoles")]
        public Task GetAllUserAsync(AddUserRolesInput input)
        {
            return userService.CreateRoleAsync(input);
        }
    }
}