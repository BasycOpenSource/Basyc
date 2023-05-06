namespace Basyc.MessageBus.NetMQ.Shared;

#pragma warning disable CA1819 // Properties should not return arrays

public record CheckInMessage
{
    public CheckInMessage(string workerId, string[] supportedMessageTypes)
    {
        WorkerId = workerId;
        SupportedMessageTypes = supportedMessageTypes ?? Array.Empty<string>();
    }

    public string[] SupportedMessageTypes { get; init; }

    public string WorkerId { get; init; }
}
