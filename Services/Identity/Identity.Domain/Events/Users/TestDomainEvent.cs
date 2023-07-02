using MediatR;

namespace Identity.Domain.Events.Users;

public class TestDomainEvent : INotification
{
    public string UserName { get; }

    public TestDomainEvent(string userName)
    {
        UserName = userName;
    }
}