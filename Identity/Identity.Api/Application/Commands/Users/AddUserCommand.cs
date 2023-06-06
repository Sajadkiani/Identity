using Identity.Domain.Aggregates.Users.Enums;
using MediatR;

namespace IdentityService.Api.Application.Commands.Users;

public class AddUserCommand : INotification
{
    public AddUserCommand(Gender gender, string password, string email, string userName, string family, string name)
    {
        Gender = gender;
        Password = password;
        Email = email;
        UserName = userName;
        Family = family;
        Name = name;
    }

    public string Name { get; }
    public string Family { get; }
    public string UserName { get; }
    public string Email { get; }
    public string Password { get; }
    public Gender Gender { get; }
}