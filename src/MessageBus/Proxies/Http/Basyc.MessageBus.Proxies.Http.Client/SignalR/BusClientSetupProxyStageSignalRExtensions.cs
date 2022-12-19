using Basyc.MessageBus.Client;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.HttpProxy.Client.Http;
using Basyc.MessageBus.HttpProxy.Shared.SignalR;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BusClientSetupProxyStageSignalRExtensions
    {
        public static BusClientUseDiagnosticsStage SelectSignalRProxyProvider(this BusClientSetupProviderStage parent, string signalRServerUri, string hubPattern = SignalRConstants.ProxyClientHubPattern)
        {
            parent.services.AddBasycSerialization()
                .SelectProtobufNet();
            parent.services.AddSingleton<IObjectMessageBusClient, SignalRProxyObjectMessageBusClient>();
            parent.services.AddSingleton<ITypedMessageBusClient, TypedFromObjectMessageBusClient>();

            parent.services.Configure<SignalROptions>(options =>
            {
                options.SignalRServerUri = signalRServerUri;
                options.ProxyClientHubPattern = hubPattern;
            });

            return new BusClientUseDiagnosticsStage(parent.services);
        }
    }
}