using System.Data;
using System.Reflection;
using AppDomain.SeedWork;
using EventBus;
using EventBus.Services;
using Identity.Domain.Aggregates.Users;
using Identity.Infrastructure.ORM.EF.Configs;
using Identity.Infrastructure.ORM.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Infrastructure.Data.EF
{
    public class AppDbContext : EventDbContext, IUnitOfWork
    {
        private readonly IMediator mediator;
        private IDbContextTransaction currentTransaction;
        public bool HasActiveTransaction => currentTransaction != null;
        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IIntegrationEventLogService integrationEventLogService,
            IMediator mediator) : base(options,integrationEventLogService, mediator)
        {
        }

        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(UserConfig)));
            base.OnModelCreating(builder);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return true;
        }
        
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (currentTransaction != null) return null;

            currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            
            if (transaction != currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await base.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (currentTransaction != null)
                {
                    currentTransaction.Dispose();
                    currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                currentTransaction?.Rollback();
            }
            finally
            {
                if (currentTransaction != null)
                {
                    currentTransaction.Dispose();
                    currentTransaction = null;
                }
            }
        }
    }
}