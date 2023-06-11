using AutoMapper;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;

namespace IdentityService.Api.MapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddUserInput, User>();
            CreateMap<AddRoleInput, Role>();
        }
    }
}