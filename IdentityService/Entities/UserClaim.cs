using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Entities;

public class UserClaim : IdentityUserClaim<Guid>
{
}