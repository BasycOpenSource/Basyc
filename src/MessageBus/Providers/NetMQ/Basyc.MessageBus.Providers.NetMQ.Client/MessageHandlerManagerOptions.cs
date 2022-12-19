namespace Basyc.MessageBus.Client.NetMQ
{
	public class MessageHandlerManagerOptions
	{
		public bool IsDiagnosticLoggingEnabled { get; set; }
		public List<NetMQMessageHandlerInfo> HandlerInfos { get; } = new List<NetMQMessageHandlerInfo>();
	}
}