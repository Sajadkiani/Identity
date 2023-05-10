using System.Threading.Tasks;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("api/users")]
public class AuthController : ControllerBase
{
    public AuthController()
    {
            
    }
    
    [HttpPost]
    public async Task<AuthViewModel.LoginOutput> LoginAsync(AuthViewModel.LoginInput input)
    {

        return new AuthViewModel.LoginOutput();
    }
        
    public async Task<AuthViewModel.LoginOutput> RefreshTokenAsync(AuthViewModel.RefreshTokenInput input)
    {

        return new AuthViewModel.LoginOutput();
    }
}