using Basyc.Diagnostics.SignalR.Shared.DTOs;

namespace Basyc.Diagnostics.SignalR.Shared
{
	public interface IReceiversMethodsServerCanCall
	{
		Task ReceiveChangesFromServer(ChangesSignalRDTO changes);
	}
}
