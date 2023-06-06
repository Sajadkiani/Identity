using System.Threading.Tasks;
using Identity.Domain.Aggregates.Users;
using IdentityService.ViewModels;

namespace Identity.Api.Infrastructure.AppServices;

public interface ITokenGeneratorService
{
    Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user);
}