using Identity.Domain.Aggregates.Users.Enums;
using Identity.Domain.Events.Users;
using Identity.Domain.Exceptions;
using Identity.Domain.IServices;
using Identity.Domain.SeedWork;
using Identity.Domain.Validations.Users;

namespace Identity.Domain.Aggregates.Users
{
    public class User : Entity, IAggregateRoot
    {
        private User()
        {
            
        }
        public User(string name, string family, string userName, string email, string password, Gender gender,
            IUserBcScopeValidation bcScopeValidation, IPasswordService passwordService)
        {
            Name = name;
            Family = family;
            UserName = userName;
            Email = email;
            Password = passwordService.HashPassword(password, userName);
            Gender = gender;
            userRoles = new List<UserRole>();
            Status = UserStatus.Active;
            //TODO: all invariants and data consistencies must put here 
            Validate(bcScopeValidation);
            AddDomainEvent(new TestDomainEvent(UserName));
        }

        public string Name { get; private set; }
        public string Family { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public Gender Gender { get; private set; }
        public UserStatus Status { get; private set; }
        private readonly List<UserRole> userRoles;

        public IReadOnlyCollection<UserRole> UserRoles => userRoles;

        private void Validate(IUserBcScopeValidation bcScopeValidation)
        {
            var isExistEmail = bcScopeValidation.IsExistEmail(Email);
            if (isExistEmail)
            {
                throw new AppBaseDomainException(AppDomainMessages.InvalidEmail);
            }
            
            var isExistUserName = bcScopeValidation.IsExistUserName(UserName);
            if (isExistUserName)
            {
                throw new AppBaseDomainException(AppDomainMessages.InvalidUserName);
            }
        }
        
        public void AddUserRole(int roleId)
        {
            var userRole = new UserRole(roleId);
            userRoles.Add(userRole);
        }
    }
}