using System.Threading.Tasks;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;

namespace Identity.Api.Infrastructure.AppServices;

public interface ITokenGeneratorService
{
    Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user);
}