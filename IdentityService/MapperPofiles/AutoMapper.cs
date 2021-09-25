using AutoMapper;
using IdentityService.Models;
using IdentityService.ViewModels;

namespace IdentityService.MapperPofiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap <AddUserVm,AddUserModel> ();
        }
    }
}