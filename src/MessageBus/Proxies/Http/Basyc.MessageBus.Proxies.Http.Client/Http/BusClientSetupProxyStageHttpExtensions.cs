using Basyc.MessageBus.Client;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.HttpProxy.Client.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static class BusClientSetupProxyStageHttpExtensions
{
    public static SetupHttpProxyStage UseHttpProxy(this BusClientSetupProviderStage builder)
    {
        builder.Services.AddBasycSerialization()
            .SelectProtobufNet();
        builder.Services.AddSingleton(new HttpClient());
        builder.Services.AddSingleton<IObjectMessageBusClient, HttpProxyObjectMessageBusClient>();
        builder.Services.AddSingleton<ITypedMessageBusClient, TypedFromObjectMessageBusClient>();

        return new SetupHttpProxyStage(builder.Services);
    }
}
