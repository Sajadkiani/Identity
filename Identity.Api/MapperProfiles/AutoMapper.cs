using AutoMapper;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using IdentityGrpcServer;

namespace Identity.Api.MapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddUserInput, User>();
            CreateMap<AddRoleInput, Role>();
            CreateMap<AuthViewModel.GetTokenOutput, RefreshTokenResponse>();
        }
    }
}