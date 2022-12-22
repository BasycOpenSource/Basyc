using System.Threading.Tasks;

namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

public interface IClientMethodsServerCanCall
{
	Task ReceiveRequestResultMetadata(RequestMetadataSignalRDTO requestMetadata);
	Task ReceiveRequestResult(ResponseSignalRDTO response);
	Task ReceiveRequestFailed(RequestFailedSignalRDTO requestFailed);
}