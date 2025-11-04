using RabbitMQ.Client;

namespace Identity.Infrastructure.MtuBus;

public interface IMtuBusConnectionManager
{
    Task<IConnection> GetConnectionAsync();
}