using Identity.Domain.SeedWork;

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