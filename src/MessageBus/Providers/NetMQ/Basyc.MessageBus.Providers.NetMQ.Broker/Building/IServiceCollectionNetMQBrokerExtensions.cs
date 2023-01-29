using Basyc.MessageBus.Broker;
using Basyc.MessageBus.Broker.NetMQ;
using Basyc.MessageBus.Broker.NetMQ.Building;
using Basyc.MessageBus.NetMQ.Shared;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionNetMqBrokerExtensions
{
	public static SelectDiagnosticStage AddBasycNetMqMessageBroker(this IServiceCollection services,
		int brokerServerPort = 5367, string brokerServerAddress = "localhost")
	{
		services.AddSingleton<IMessageBrokerServer, NetMqMessageBrokerServer>();
		services.AddSingleton<IWorkerRegistry, WorkerRegistry>();

		services.AddBasycSerialization()
			.SelectProtobufNet();

		services.AddSingleton<INetMqMessageWrapper, NetMqMessageWrapper>();
		services.Configure<NetMqMessageBrokerServerOptions>(x =>
		{
			x.BrokerServerAddress = brokerServerAddress;
			x.BrokerServerPort = brokerServerPort;
		});
		return new SelectDiagnosticStage(services);
	}
}
