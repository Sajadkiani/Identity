using Identity.Domain.SeedWork;
using Identity.Infrastructure.ORM.EF;

namespace Identity.Infrastructure.EF.Stores;

public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IAggregateRoot
{
    protected readonly AppDbContext context;

    public IUnitOfWork UnitOfWork => context;

    public Repository(
        AppDbContext context
    )
    {
        this.context = context;
    }

    public Task<TEntity?> GetByIdAsync(TId id)
    {
        return context.Set<TEntity>().FindAsync(id).AsTask();
    }
}