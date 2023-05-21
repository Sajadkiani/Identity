using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityService.Entities;
using IdentityService.ViewModels;

namespace IdentityService.Services;

public interface ITokenGeneratorService
{
    Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user);
}