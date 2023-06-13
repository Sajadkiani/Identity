using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using Identity.Domain.Validations.Users;
using MediatR;

namespace Identity.Api.Application.Commands.Users;

public class AddUserCommandHandler : IRequestHandler<AddUserCommand, int>
{
    private readonly IUserStore userStore;
    private readonly IUserBcScopeValidation validation;
    private readonly IPasswordService passwordService;

    public AddUserCommandHandler(
        IUserStore userStore,
        IUserBcScopeValidation validation,
        IPasswordService passwordService
        )
    {
        this.userStore = userStore;
        this.validation = validation;
        this.passwordService = passwordService;
    }
    public async Task<int> Handle(AddUserCommand notification, CancellationToken cancellationToken)
    {
        var user = new User(notification.Name, notification.Family, notification.UserName, notification.Email,
            notification.Password, notification.Gender, validation, passwordService);

        await userStore.AddUserAsync(user);
        await userStore.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        
        return user.Id;
    }
}