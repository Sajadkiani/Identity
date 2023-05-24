using System;
using System.Threading.Tasks;

namespace IdentityService.ViewModels;

public class AuthViewModel
{
    public class AddTokenInput
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireDate { get; set; }
        public Guid UserId { get; set; }
    }
    
    public class LoginInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class GetTokenOutput
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }

    public class RefreshTokenInput
    {
        public string RefreshToken { get; set; }
    }
}