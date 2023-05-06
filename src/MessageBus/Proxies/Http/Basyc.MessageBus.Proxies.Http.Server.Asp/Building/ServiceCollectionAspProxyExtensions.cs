using Basyc.MessageBus.HttpProxy.Server.Asp.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionAspProxyExtensions
{
    public static SelectProxyStage AddBasycMessageBusProxy(this IServiceCollection services) => new(services);
}
