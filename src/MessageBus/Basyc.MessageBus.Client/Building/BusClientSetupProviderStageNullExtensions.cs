using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Client.Building;

public static class BusClientSetupProviderStageNullExtensions
{
    public static BusClientUseDiagnosticsStage SelectNullClient(this BusClientSetupProviderStage parent)
    {
        parent.Services.AddSingleton<IObjectMessageBusClient, NullObjectMessageBusClient>();
        parent.Services.AddSingleton<ITypedMessageBusClient, NullTypedMessageBusClient>();
        return new BusClientUseDiagnosticsStage(parent.Services);
    }
}
