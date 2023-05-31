using System.Threading.Tasks;
using Identity.Domain.Aggregates.Users;
using IdentityService.ViewModels;

namespace IdentityService.Api.AppServices;

public interface ITokenGeneratorService
{
    Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user);
}