using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Application.Queries.Users;
using Identity.Api.ViewModels;
using Identity.Infrastructure.MtuBus;
using Identity.Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICurrentUser currentUser;
        private readonly IDomainEventDispatcher eventBus;
        private readonly IMediator mediator;

        public UserController(
            IMapper mapper,
            ICurrentUser currentUser,
            IDomainEventDispatcher eventBus,
            IMediator mediator
        )
        {
            this.mapper = mapper;
            this.currentUser = currentUser;
            this.eventBus = eventBus;
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task AddUserAsync([FromBody] AddUserInput input)
        {
            await eventBus.SendAsync(new AddUserCommand(input.Gender, input.Password, input.Email,
                input.UserName, input.Family, input.Name));
        }

        [HttpGet("{userId}/roles")]
        [Authorize]
        public async Task<IEnumerable<AuthViewModel.UserRoleOutput>> GetUserRolesAsync([FromRoute] int userId)
        {
            return await eventBus.SendAsync(new GetUserRolesQuery(userId));
        }
        
        
        [HttpGet("test")]
        [Authorize]
        [RequiredClaims("string")]
        public async Task test()
        {
            var request = HttpContext.Request.HttpContext.User;
        }
    }
}