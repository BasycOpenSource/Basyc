namespace Basyc.MessageBus.Client.NetMQ;

public class MessageHandlerManagerOptions
{
	public bool IsDiagnosticLoggingEnabled { get; set; }
	public List<NetMqMessageHandlerInfo> HandlerInfos { get; } = new();
}
