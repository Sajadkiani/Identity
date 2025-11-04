using System.Text;
using Events;
using Identity.Domain.Events.Users;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Identity.Infrastructure.MtuBus.Consumers;

public class TestConsumer : IMtuConsumer
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<TestConsumer> _logger;

    public string QueueName => "test-queue-name";

    public TestConsumer(
        IDomainEventDispatcher eventDispatcher,
        ILogger<TestConsumer> logger)
    {
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task HandleAsync(string messageJson, CancellationToken cancellationToken)
    {
        var message = JsonSerializer.Deserialize<TestIntegrationEvent>(messageJson);
        if (message == null)
        {
            _logger.LogWarning("Received null message for {QueueName}", QueueName);
            return;
        }

        _logger.LogInformation("Processing message for {QueueName}: {UserName}", QueueName, message.UserName);

        await _eventDispatcher.DispatchAsync(new TestDomainEvent(message.UserName));
    }
}

public abstract class MtuBusHostedService : BackgroundService
{
    private readonly ILogger<MtuBusHostedService> _logger;
    private readonly IEnumerable<IMtuConsumer> _consumers;
    private readonly IMtuBusConnectionManager _connectionManager;
    private IChannel? _channel;


    protected MtuBusHostedService(
        string  queueName,
        IMtuBusConnectionManager connectionManager,
        ILogger<MtuBusHostedService> logger,
        IEnumerable<IMtuConsumer> consumers)
    {
        _logger = logger;
        _consumers = consumers;
        _connectionManager = connectionManager;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var connection = await _connectionManager.GetConnectionAsync();
        _channel = await connection.CreateChannelAsync(null, cancellationToken);

        foreach (var consumer in _consumers)
        {
            await _channel.QueueDeclareAsync(
                queue: consumer.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var rabbitConsumer = new AsyncEventingBasicConsumer(_channel);

            rabbitConsumer.ReceivedAsync += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    await consumer.HandleAsync(json, cancellationToken);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from {QueueName}: {Json}", consumer.QueueName, json);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false, cancellationToken);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: consumer.QueueName,
                autoAck: false,
                consumer: rabbitConsumer,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Consumer for queue {QueueName} started", consumer.QueueName);
        }

        _logger.LogInformation("MTU bus started with {Count} consumers.", _consumers.Count());
    }
}