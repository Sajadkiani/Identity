using MediatR;

namespace Identity.Domain.Events.Users;

public class SampleDomainEvent : INotification
{
    public SampleDomainEvent(string name)
    {
        Name = name;
    }

    public string Name { get; }
}