using Basyc.MessageBus.Client;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.HttpProxy.Client.Http;
using Basyc.Serialization.Abstraction;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static class BusClientSetupProxyStageHttpExtensions
{
	public static SetupHttpProxyStage UseHttpProxy(this BusClientSetupProviderStage builder)
	{
		builder.services.AddBasycSerialization()
			.SelectProtobufNet();
		builder.services.AddSingleton(new HttpClient());
		builder.services.AddSingleton<IObjectMessageBusClient, HttpProxyObjectMessageBusClient>();
		builder.services.AddSingleton<ITypedMessageBusClient, TypedFromObjectMessageBusClient>();

		return new SetupHttpProxyStage(builder.services);
	}
}