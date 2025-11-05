using Identity.Api.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Identity.Infrastructure.MtuBus;

public class MtuBusConnectionManager : IMtuBusConnectionManager, IDisposable
{
    private readonly AppOptions.MtuRabbitMqOptions _options;
    private readonly ILogger<MtuBusConnectionManager> _logger;
    private IConnection? _connection;

    public MtuBusConnectionManager(
        IOptions<AppOptions.MtuRabbitMqOptions> options,
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

        _logger.LogInformation($"Creating new MTU bus connection to {_options.HostName}.");
        _connection = await factory.CreateConnectionAsync();
        return _connection;
    }

    public void Dispose()
    {
        if (_connection is { IsOpen: true })
        {
            _logger.LogInformation("Closing MTU bus connection...");
            _connection.CloseAsync().GetAwaiter().GetResult();
            _connection.Dispose();
        }
    }
}