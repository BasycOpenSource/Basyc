using System.Threading.Tasks;

namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

public interface IMethodsClientCanCall
{
    Task Request(RequestSignalRDTO proxyRequest);
}
