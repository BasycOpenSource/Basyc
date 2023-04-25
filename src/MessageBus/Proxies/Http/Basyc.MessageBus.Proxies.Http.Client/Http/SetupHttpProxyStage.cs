using Basyc.DependencyInjection;
using Basyc.MessageBus.Client.Building;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.HttpProxy.Client.Http;

public class SetupHttpProxyStage : BuilderStageBase
{
    public SetupHttpProxyStage(IServiceCollection services) : base(services)
    {
    }

    public BusClientUseDiagnosticsStage SetProxyServerUri(Uri hostUri)
    {
        Services.Configure<HttpProxyObjectMessageBusClientOptions>(x => x.ProxyHostUri = hostUri);
        return new(Services);
    }
}
