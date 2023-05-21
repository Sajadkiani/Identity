using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Entities
{
    
    public class UserClaim : IdentityUserClaim<Guid>
    {
        
    }
    public class User : IdentityUser<Guid>
    {

        public ICollection<Token> Tokens { get; set; }
    }
}