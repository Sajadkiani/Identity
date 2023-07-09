using System;

namespace Identity.Api.ViewModels;

public class AuthViewModel
{
    public class LoginInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserRoleOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
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