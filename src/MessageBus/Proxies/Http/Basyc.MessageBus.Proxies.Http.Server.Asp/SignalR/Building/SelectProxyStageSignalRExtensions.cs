using Basyc.MessageBus.HttpProxy.Server.Asp.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class SelectProxyStageSignalRExtensions
{
    public static void UseSignalRProxy(this SelectProxyStage parent)
    {
        parent.services.AddSignalR();
    }
}
