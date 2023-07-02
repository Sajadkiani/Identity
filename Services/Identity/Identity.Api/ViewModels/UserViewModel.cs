using System;
using Identity.Domain.Aggregates.Users.Enums;

namespace Identity.Api.ViewModels
{
    public class AddUserRolesInput
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; }
    }
    
    public class AddRoleInput
    {
        public string Name { get; set; }
    }
    
    public class AddUserInput
    {
        public Gender Gender { get; set; }
        public string Family { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}