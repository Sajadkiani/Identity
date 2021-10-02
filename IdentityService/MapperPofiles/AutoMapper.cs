using AutoMapper;
using IdentityService.Entities;
using IdentityService.Models;
using IdentityService.ViewModels;

namespace IdentityService.MapperPofiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddUserVm, User>();
            CreateMap<User, GetUserModel>();
        }
    }
}