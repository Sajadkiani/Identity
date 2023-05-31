using Identity.Domain.Aggregates.Users.Enums;
using Identity.Domain.SeedWork;

namespace Identity.Domain.Aggregates.Users
{
    public class User : Entity
    {
        public User(string name, string family, string userName, string email, string password, Gender gender)
        {
            Name = name;
            Family = family;
            UserName = userName;
            Email = email;
            Password = password;
            Gender = gender;
            userRoles = new List<UserRole>();
            tokens = new List<Token>();
        }

        public string Name { get; private set; }
        public string Family { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public Gender Gender { get; private set; }
        private readonly List<UserRole> userRoles;

        public IReadOnlyCollection<UserRole> UserRoles => userRoles;

        private readonly List<Token> tokens;
        public IReadOnlyCollection<Token> Tokens => tokens;
        
        public void AddTokens(string accessToken, string refreshToken, DateTime expireDate)
        {
            var token = new Token(accessToken, refreshToken, expireDate);
            tokens.Add(token);
        }
        
        public void AddUserRole(int roleId)
        {
            var userRole = new UserRole(roleId);
            userRoles.Add(userRole);
        }
    }
}