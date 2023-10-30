using System;
using Identity.Domain.Aggregates.Users.Enums;

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

    public class GetUserByUserNameOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Gender Gender { get; set; }
        public UserStatus Status { get;  set; }
    }

    public class RefreshTokenInput
    {
        public string RefreshToken { get; set; }
    }
}