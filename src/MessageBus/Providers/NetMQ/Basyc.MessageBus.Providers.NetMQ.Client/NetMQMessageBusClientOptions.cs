using Basyc.MessageBus.Client.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client.NetMQ;

public class NetMQMessageBusClientOptions
{
	public int BrokerServerPort { get; set; }
	public string? BrokerServerAddress { get; set; }
	public string? WorkerId { get; set; }
}