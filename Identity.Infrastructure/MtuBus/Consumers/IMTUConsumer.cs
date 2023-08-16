using System.Text;
using System.Text.Json.Serialization;
using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Identity.Infrastructure.MtuBus.Consumers;

public interface IMTUConsumer
{
    Task StartAsync(IConnection connection, CancellationToken cancellationToken);
}

 public class MTUConsumer : IMTUConsumer
    {
        private readonly ILogger<TestIntegrationEventConsumer> _logger;
        private readonly IEventBus _eventBus;
        private const string QueueName = "test.integration.event.queue";

        public TestIntegrationEventConsumer(
            ILogger<TestIntegrationEventConsumer> logger,
            IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task StartAsync(IConnection connection, CancellationToken cancellationToken)
        {
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    var message = JsonConvert.<TestIntegrationEvent>(json);
                    if (message == null)
                        throw new InvalidOperationException("Message is null after deserialization.");

                    _logger.LogInformation("Received TestIntegrationEvent for User: {UserName}", message.UserName);

                    var command = new TestCommand { UserName = message.UserName };
                    var requestId = Guid.NewGuid();
                    var identifiedCommand = new IdentifiedCommand<TestCommand, bool>(command, requestId);

                    var result = await _eventBus.SendMediator(identifiedCommand);

                    if (result)
                        _logger.LogInformation("Successfully processed event for {UserName}", message.UserName);
                    else
                        _logger.LogWarning("Handler returned false for {UserName}", message.UserName);

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message: {Json}", json);
                    await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync(QueueName, autoAck: false, consumer: consumer);

            _logger.LogInformation("Consumer for {QueueName} started", QueueName);
        }
    }