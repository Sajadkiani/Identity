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
            CreateMap<AddUserInput, User>();
            CreateMap<AddRoleInput, Role>();
            CreateMap<AuthViewModel.AddTokenInput, Token>();
            CreateMap<AuthViewModel.GetTokenOutput , AuthViewModel.AddTokenInput>();
        }
    }
}