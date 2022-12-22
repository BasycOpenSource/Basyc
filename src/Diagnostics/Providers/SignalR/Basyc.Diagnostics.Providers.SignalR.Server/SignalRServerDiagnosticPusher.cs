using Basyc.Diagnostics.Server.Abstractions;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.Diagnostics.SignalR.Shared;
using Basyc.Diagnostics.SignalR.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace Basyc.Diagnostics.SignalR.Server;

public class SignalRServerDiagnosticPusher : IServerDiagnosticPusher
{

	private readonly IHubContext<LoggingReceiversHub, IReceiversMethodsServerCanCall> receiversHubContext;

	public SignalRServerDiagnosticPusher(IHubContext<LoggingReceiversHub, IReceiversMethodsServerCanCall> receiversHubContext)
	{
		this.receiversHubContext = receiversHubContext;
	}

	public Task PushChangesToReceivers(DiagnosticChanges changes)
	{
		var changeDTO = ChangesSignalRDTO.ToDto(changes);
		return receiversHubContext.Clients.All.ReceiveChangesFromServer(changeDTO);

	}
}