using Identity.Domain.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Aggregates.Users;

public class UserRole : Entity
{
    public UserRole(int roleId)
    {
        RoleId = roleId;
    }

    public int UserId { get; private set; }
    public User User { get; private set; }
    
    public int RoleId { get; private set; }
    public Role Role { get; private set; }
}

public class Role : Entity
{
    public Role(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    private List<UserRole> _userRoles;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;
}