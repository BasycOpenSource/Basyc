namespace Basyc.MessageBus.Client.NetMQ;

public class NetMqMessageBusClientOptions
{
    public int BrokerServerPort { get; set; }
    public string? BrokerServerAddress { get; set; }
    public string? WorkerId { get; set; }
}
