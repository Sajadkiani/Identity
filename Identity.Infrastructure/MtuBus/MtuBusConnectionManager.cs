using Identity.Api.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Identity.Infrastructure.MtuBus;

internal class MtuBusConnectionManager : IDisposable
{
    private readonly AppOptions.MTuRabbitMqOptions _options;
    private readonly ILogger<MtuBusConnectionManager> _logger;
    private IConnection? _connection;

    public MtuBusConnectionManager(
        IOptions<AppOptions.MTuRabbitMqOptions> options,
        ILogger<MtuBusConnectionManager> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
        };

        _logger.LogInformation($"Creating new RabbitMQ connection to {_options.HostName}");
        _connection = await factory.CreateConnectionAsync();
        return _connection;
    }

    public void Dispose()
    {
        if (_connection is { IsOpen: true })
        {
            _logger.LogInformation("Closing RabbitMQ connection...");
            _connection.CloseAsync().GetAwaiter().GetResult();
            _connection.Dispose();
        }
    }
}