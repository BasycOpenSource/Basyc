namespace Basyc.MessageBus.Broker.NetMQ;
#pragma warning disable CA1002 // Do not expose generic lists

public class MessageTypeRecord
{
    public MessageTypeRecord(string messageType, List<string> workerAddresses, int lastUsedWorker)
    {
        MessageType = messageType;
        WorkerIds = workerAddresses;
        LastUsedWorkerId = lastUsedWorker;
    }

    public string MessageType { get; }

    public List<string> WorkerIds { get; }

    public int LastUsedWorkerId { get; set; }
}
