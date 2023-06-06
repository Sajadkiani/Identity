using Identity.Domain.SeedWork;

namespace Identity.Infrastructure.EF.Stores;

public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IAggregateRoot
{
    private readonly AppDbContext context;
    public IUnitOfWork UnitOfWork { get; }

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