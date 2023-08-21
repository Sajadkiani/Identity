using Identity.Domain.Exceptions;
using Identity.Infrastructure.ORM.EF;

namespace Identity.Infrastructure.EF.Idempotency;

public class RequestManager : IRequestManager
{
    private readonly AppDbContext context;

    public RequestManager(AppDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<bool> ExistAsync(Guid id)
    {
        var request = await context.
            FindAsync<ClientRequest>(id);

        return request != null;
    }

    public async Task CreateRequestForCommandAsync<T>(Guid id)
    {
        var exists = await ExistAsync(id);

        var request = exists ?
            throw new AppInternalDomainException(($"Request with {id} already exists")) :
            new ClientRequest()
            {
                Id = id,
                Name = typeof(T).Name,
                Time = DateTime.UtcNow
            };

        context.Add(request);

        await context.SaveChangesAsync();
    }
}
