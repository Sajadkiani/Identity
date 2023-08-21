using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Identity.Infrastructure.EF;
using Identity.Infrastructure.Exceptions;
using Identity.Infrastructure.ORM.EF;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ApplicationException = Identity.Infrastructure.Exceptions.ApplicationException;

namespace Identity.Api.Application.IntegrationEvents;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory;
    private readonly IEventBus eventBus;
    private readonly AppDbContext context;
    private readonly IIntegrationEventLogService eventLogService;
    private readonly ILogger<IntegrationEventService> logger;
    private readonly IEventInitializer eventInitializer;

    public IntegrationEventService(
        IEventBus eventBus,
        AppDbContext context,
        IntegrationEventLogContext eventLogContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
        ILogger<IntegrationEventService> logger,
        IEventInitializer eventInitializer
        )
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        integrationEventLogServiceFactory = integrationEventLogServiceFactory ??
                                             throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        eventLogService = integrationEventLogServiceFactory(context.Database.GetDbConnection());
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.eventInitializer = eventInitializer;
    }

    //TODO: later add a .net worker to use this method and constantly send events.
    public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
    {
        var pendingLogEvents = await eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);
        var eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
            .GetTypes()
            .Where(t => t.Name.EndsWith("IntegrationEvent"))
            .ToList();
        
        foreach (var logEvt in pendingLogEvents)
        {
            logger.LogInformation(
                "----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                logEvt.EventId, Program.AppName, logEvt.Content);

            var eventType = eventTypes.FirstOrDefault(item => item.Name == logEvt.EventTypeName);
            if (eventType is null)
            {
                throw new ApplicationException.Internal(AppMessages.InternalError);
            }

            var deserializedEvent = JsonSerializer.Deserialize(logEvt.Content, eventType);
            try
            {
                await eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                await eventBus.Publish(deserializedEvent);
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

    public async Task AddAndSaveEventAsync<TEvent>(TEvent evt)
    {
        logger.LogInformation(
            "----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})",
            evt, evt);

        await eventLogService.SaveEventAsync(evt, context.GetCurrentTransaction());
    }
}