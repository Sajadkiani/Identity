using System;

namespace IdentityService.Models
{
    public class GetUserModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}