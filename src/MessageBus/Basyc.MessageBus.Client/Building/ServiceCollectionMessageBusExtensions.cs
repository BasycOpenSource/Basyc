using Basyc.MessageBus.Client;
using Basyc.MessageBus.Client.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionMessageBusExtensions
{
    public static BusClientSetupHandlersStage AddBasycMessageBus(this IServiceCollection services)
    {
        services.AddSingleton<ISharedRequestIdCounter, InMemorySharedRequestIdCounter>();
        return new BusClientSetupHandlersStage(services);
    }
}
