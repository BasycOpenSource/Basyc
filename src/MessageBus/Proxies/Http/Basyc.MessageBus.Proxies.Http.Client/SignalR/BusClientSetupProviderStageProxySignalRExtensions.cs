using Basyc.MessageBus.Client;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.HttpProxy.Client.Http;
using Basyc.MessageBus.HttpProxy.Shared.SignalR;

namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA1054 // URI-like parameters should not be strings

public static class BusClientSetupProviderStageProxySignalRExtensions
{
    public static BusClientUseDiagnosticsStage SelectSignalRProxyProvider(this BusClientSetupProviderStage parent,
        string signalRServerUri,
        string hubPattern = SignalRConstants.ProxyClientHubPattern)
    {
        parent.Services.AddBasycSerialization()
            .SelectProtobufNet();
        parent.Services.AddSingleton<IObjectMessageBusClient, SignalRProxyObjectMessageBusClient>();
        parent.Services.AddSingleton<ITypedMessageBusClient, TypedFromObjectMessageBusClient>();

        parent.Services.Configure<SignalROptions>(options =>
        {
            options.SignalRServerUri = signalRServerUri;
            options.ProxyClientHubPattern = hubPattern;
        });

        return new BusClientUseDiagnosticsStage(parent.Services);
    }
}
