using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Services;
using Identity.Api.Extensions;
using Identity.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;

namespace Identity.Api.Application.Behaviors;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> logger;

    private readonly AppDbContext dbContext;
    private readonly IIntegrationEventLogService _eventLogService;

    public TransactionBehaviour(
        AppDbContext dbContext,
        IIntegrationEventLogService eventLogService,
        ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
        _eventLogService = eventLogService;
        this.logger = logger ?? throw new ArgumentException(nameof(ILogger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = default(TResponse);
        var typeName = request.GetGenericTypeName();

        try
        {
            if (dbContext.HasActiveTransaction)
            {
                return await next();
            }

            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                await using var transaction = await dbContext.BeginTransactionAsync();
                using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                {
                    logger.LogInformation(
                        $"----- Begin transaction {transaction.TransactionId.ToString()} for {typeName} ({JsonConvert.SerializeObject(request)})");

                    response = await next(cancellationToken);

                    logger.LogInformation($"----- Commit transaction {transaction.TransactionId} for {typeName}");

                    await dbContext.CommitTransactionAsync(transaction);

                    transactionId = transaction.TransactionId;
                }

                await _eventLogService.PublishEventsThroughEventBusAsync(transactionId);
            });

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error handling transaction for {typeName} ({JsonConvert.SerializeObject(request)})");

            throw;
        }
    }
}