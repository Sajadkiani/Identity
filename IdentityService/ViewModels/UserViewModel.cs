using System;
using System.Collections.Generic;

namespace IdentityService.ViewModels
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
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}