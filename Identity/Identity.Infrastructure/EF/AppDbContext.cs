using System.Reflection;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.SeedWork;
using Identity.Infrastructure.EF.Configs;
using Identity.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.EF
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator mediator;

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
            : base(options)
        {
            this.mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(TokenConfig)));
            base.OnModelCreating(builder);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection.
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB. This makes
            // a single transaction including side effects from the domain event
            // handlers that are using the same DbContext with Scope lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB. This makes
            // multiple transactions. You will need to handle eventual consistency and
            // compensatory actions in case of failures.
            await mediator.DispatchDomainEventsAsync(this);

            // After this line runs, all the changes (from the Command Handler and Domain
            // event handlers) performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken) > 0;
            return result;
        }

        public DbSet<User> Users { get; set; }
    }
}