namespace Common.Services.Brokers;

public interface IBroker
{
    Task PublishAsync<TEvent>(TEvent @event);
}