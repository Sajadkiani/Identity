using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EventBus.Abstractions;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Application.Queries.Users;
using Identity.Api.Infrastructure.Brokers;
using Identity.Api.Infrastructure.Services;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/users")]
    // [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUser currentUser;
        private readonly IEventBus eventHandler;

        public UserController(
            IMapper mapper,
            ICurrentUser currentUser,
            IEventBus eventHandler
        )
        {
            this.mapper = mapper;
            this.currentUser = currentUser;
            this.eventHandler = eventHandler;
        }

        [HttpPost]
        public async Task AddUserAsync([FromBody] AddUserInput input)
        {
            await eventHandler.SendMediator(new AddUserCommand(input.Gender, input.Password, input.Email,
                input.UserName, input.Family, input.Name));
        }
        
        [HttpGet("{userId}/roles")]
        public async Task<IEnumerable<AuthViewModel.UserRoleOutput>> GetUserRolesAsync([FromRoute] int userId)
        {
            return await eventHandler.SendMediator(new GetUserRolesQuery(userId));
        }
        
        // [HttpPost("roles")]
        // public async Task AddRoleAsync([FromBody] AddRoleInput input)
        // {
        //     await eventHandler.SendMediator(new GetUserRolesQuery(input.);
        // }
        
        // [HttpPost("userRoles")]
        // public Task GetAllUserAsync(AddUserRolesInput input)
        // {
        //     await eventHandler.SendMediator(new GetUserRolesQuery(input.UserId));
        // }
    }
}