using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Entities;

public class Token : IdentityUserToken<Guid>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireDate { get; set; }
    public override string LoginProvider { get; set; } = "myapp";
    public override string Name { get; set; } = "default";
}