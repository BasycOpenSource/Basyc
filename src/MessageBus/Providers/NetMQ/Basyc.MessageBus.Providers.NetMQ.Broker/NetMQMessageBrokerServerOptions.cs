namespace Basyc.MessageBus.Broker.NetMQ;

public class NetMqMessageBrokerServerOptions
{
	public string BrokerServerAddress { get; set; } = "localhost";
	public int BrokerServerPort { get; set; } = 5553;
}
