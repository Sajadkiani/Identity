using System.Threading.Tasks;
using AutoMapper;
using IdentityService.Data.Stores.Users;
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

        [HttpGet("{userId}")]
        public async Task AddUserAsync(AddUserVm vm)
        {
            var model=mapper.Map<AddUserVm>(vm);
            
        }
    }
}