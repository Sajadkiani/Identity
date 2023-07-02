using Identity.Domain.SeedWork;
using Identity.Infrastructure.EF;
using MediatR;

namespace Identity.Infrastructure.Extensions;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, AppDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(item => item.Entity.DomainEvents != null)
            .Where(x => x.Entity.DomainEvents.Any());

        if (domainEntities.Any() == false)
        {
            return;
        }

        var domainEvents = domainEntities
            .Select(x => x.Entity.DomainEvents)
            .ToList();
        
        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());
        
        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
