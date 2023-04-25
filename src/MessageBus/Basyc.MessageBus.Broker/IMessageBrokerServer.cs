namespace Basyc.MessageBus.Broker;

public interface IMessageBrokerServer : IDisposable
{
    void Start();

    Task StartAsync(CancellationToken cancellationToken = default);
}
