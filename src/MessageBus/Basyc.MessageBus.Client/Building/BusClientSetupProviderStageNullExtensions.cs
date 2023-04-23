using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Client.Building;

public static class BusClientSetupProviderStageNullExtensions
{
    public static BusClientUseDiagnosticsStage SelectNullClient(this BusClientSetupProviderStage parent)
    {
        parent.services.AddSingleton<IObjectMessageBusClient, NullObjectMessageBusClient>();
        parent.services.AddSingleton<ITypedMessageBusClient, NullTypedMessageBusClient>();
        return new BusClientUseDiagnosticsStage(parent.services);
    }
}
