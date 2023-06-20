using System;
using System.Data.Common;
using System.Threading.Tasks;
using EventBus.Events;
using Identity.Infrastructure.EF;
using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.Extensions.Logging;
using IEventBus = EventBus.Abstractions.IEventBus;

namespace Identity.Api.Application.IntegrationEvents;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory;
    private readonly IEventBus eventBus;
    private readonly AppDbContext context;
    private readonly IIntegrationEventLogService eventLogService;
    private readonly ILogger<IntegrationEventService> logger;

    public IntegrationEventService(
        IEventBus eventBus,
        AppDbContext context,
        IntegrationEventLogContext eventLogContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
        ILogger<IntegrationEventService> logger
        )
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        integrationEventLogServiceFactory = integrationEventLogServiceFactory ??
                                             throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        eventLogService = integrationEventLogServiceFactory(context.Database.GetDbConnection());
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
    {
        var pendingLogEvents = await eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

        foreach (var logEvt in pendingLogEvents)
        {
            logger.LogInformation(
                "----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

            try
            {
                await eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                await eventBus.Publish(logEvt.IntegrationEvent);
                await eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}",
                    logEvt.EventId, Program.AppName);

                await eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
            }
        }
    }

    public async Task AddAndSaveEventAsync(IntegrationEvent evt)
    {
        logger.LogInformation(
            "----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);
        
        await eventLogService.SaveEventAsync(evt, context.GetCurrentTransaction());
    }
}