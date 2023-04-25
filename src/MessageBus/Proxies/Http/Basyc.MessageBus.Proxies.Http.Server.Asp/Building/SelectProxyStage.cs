using Basyc.DependencyInjection;

namespace Basyc.MessageBus.HttpProxy.Server.Asp.Building;

public class SelectProxyStage : BuilderStageBase
{
    public SelectProxyStage(IServiceCollection services) : base(services)
    {
    }
}
