using System.Threading.Tasks;
using Identity.Domain.Aggregates.Users;
using IdentityService.ViewModels;

namespace IdentityService.Services;

public interface ITokenService
{
    Task<Token> GetTokenByRefreshAsync(string refresh);
    Task AddTokenAsync(AuthViewModel.AddTokenInput input);
}