namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

public interface IClientMethodsServerCanCall
{
	Task ReceiveRequestResultMetadata(RequestMetadataSignalRDto requestMetadata);
	Task ReceiveRequestResult(ResponseSignalRDto response);
	Task ReceiveRequestFailed(RequestFailedSignalRDto requestFailed);
}
