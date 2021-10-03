using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using IdentityService.Data.Stores.Users;
using IdentityService.Entities;
using IdentityService.Models;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserStore userStore;
        private readonly IMapper mapper;

        public UserController(
            IUserStore userStore,
            IMapper mapper
        )
        {
            this.userStore = userStore;
            this.mapper = mapper;
        }

        [HttpGet("{userId}")]
        public async Task<GetUserModel> GetUserAsync(int userId)
        {
            var user = await userStore.GetUserAsync(userId);
            return new GetUserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password
            };
        }

        [HttpPost]
        public async Task AddUserAsync(AddUserVm vm)
        {
            var user = mapper.Map<User>(vm);
            await userStore.AddUserAsync(user);
            await userStore.SaveChangeAsync();
        }

        [HttpGet("all")]
        public async Task<List<GetUserModel>> GetAllUserAsync()
        {
            var users = await userStore.GetUsersAsync();
            return mapper.Map<List<GetUserModel>>(users);
        }
    }
}