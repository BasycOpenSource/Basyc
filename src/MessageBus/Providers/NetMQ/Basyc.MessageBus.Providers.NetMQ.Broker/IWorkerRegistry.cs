using System.Diagnostics.CodeAnalysis;

namespace Basyc.MessageBus.Broker.NetMQ;

public interface IWorkerRegistry
{
    bool TryGetWorkersFor(string messageType, [NotNullWhen(true)] out string[] workerIds);

    void RegisterWorker(string workerId, string[] suppportedMessages);

    bool TryGetWorkerFor(string messageType, [NotNullWhen(true)] out string? workerId);
}
