using System.Threading.Tasks;

namespace IdentityService.ViewModels;

public class AuthViewModel
{
    public class LoginInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginOutput
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpireDate { get; set; }
    }

    public class RefreshTokenInput
    {
    }
}