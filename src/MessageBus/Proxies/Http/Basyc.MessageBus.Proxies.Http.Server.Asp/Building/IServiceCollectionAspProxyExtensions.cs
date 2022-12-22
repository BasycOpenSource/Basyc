using Basyc.MessageBus.HttpProxy.Server.Asp.Building;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAspProxyExtensions
{
    public static SelectProxyStage AddBasycMessageBusProxy(this IServiceCollection services)
    {
        return new SelectProxyStage(services);
    }
}
