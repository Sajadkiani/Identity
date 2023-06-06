using AutoMapper;
using Identity.Domain.Aggregates.Users;
using IdentityService.ViewModels;

namespace IdentityService.Api.MapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddUserInput, User>();
            CreateMap<AddRoleInput, Role>();
            CreateMap<AuthViewModel.AddTokenInput, Token>();
            CreateMap<AuthViewModel.GetTokenOutput , AuthViewModel.AddTokenInput>();
        }
    }
}