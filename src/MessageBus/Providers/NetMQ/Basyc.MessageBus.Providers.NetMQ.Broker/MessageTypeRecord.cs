namespace Basyc.MessageBus.Broker.NetMQ;

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
