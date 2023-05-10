using Microsoft.AspNetCore.Identity;

namespace IdentityService.Entities;

public class Role : IdentityRole
{
    public int Id { get; set; }
    public int Name { get; set; }
}