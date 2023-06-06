using System.Threading.Tasks;
using AutoMapper;
using Identity.Api.Infrastructure.AppServices;
using Identity.Domain.Aggregates.Users;
using IdentityService.Services;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IRoleService roleService;
        private readonly ICurrentUser currentUser;

        public UserController(
            IUserService userService,
            IMapper mapper,
            IRoleService roleService,
            ICurrentUser currentUser
        )
        {
            this.userService = userService;
            this.mapper = mapper;
            this.roleService = roleService;
            this.currentUser = currentUser;
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