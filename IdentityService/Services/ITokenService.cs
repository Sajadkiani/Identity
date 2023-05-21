using System.Threading.Tasks;
using IdentityService.Entities;
using IdentityService.ViewModels;

namespace IdentityService.Services;

public interface ITokenService
{
    Task<Token> GetTokenByRefreshAsync(string refresh);
    Task AddTokenAsync(AuthViewModel.AddTokenInput input);
}