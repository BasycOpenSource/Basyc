using Basyc.MessageBus.Broker;
using Basyc.MessageBus.Broker.NetMQ;
using Basyc.MessageBus.Broker.NetMQ.Building;
using Basyc.MessageBus.NetMQ.Shared;
using Basyc.Serialization.Abstraction;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionNetMQBrokerExtensions
{
	public static SelectDiagnosticStage AddBasycNetMQMessageBroker(this IServiceCollection services,
		int brokerServerPort = 5367, string brokerServerAddress = "localhost")
	{
		services.AddSingleton<IMessageBrokerServer, NetMQMessageBrokerServer>();
		services.AddSingleton<IWorkerRegistry, WorkerRegistry>();

		services.AddBasycSerialization()
			.SelectProtobufNet();

		services.AddSingleton<INetMQMessageWrapper, NetMQMessageWrapper>();
		services.Configure<NetMQMessageBrokerServerOptions>(x =>
		{
			x.BrokerServerAddress = brokerServerAddress;
			x.BrokerServerPort = brokerServerPort;
		});
		return new(services);
	}
}