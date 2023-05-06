namespace Basyc.MessageBus.Client.NetMQ;

public class MessageHandlerManagerOptions
{
    public bool IsDiagnosticLoggingEnabled { get; set; }

    public ICollection<NetMqMessageHandlerInfo> HandlerInfos { get; init; } = new List<NetMqMessageHandlerInfo>();
}
