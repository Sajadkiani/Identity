using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Entities;

public class Token : IdentityUserToken<Guid>
{
    // public Guid Id { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireDate { get; set; }

    public override string LoginProvider { get; set; }

    // public Guid UserId { get; set; }
    public User User { get; set; }
}