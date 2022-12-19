using Basyc.Diagnostics.Server.Abstractions;
using Basyc.Diagnostics.SignalR.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Basyc.Diagnostics.SignalR.Server
{
	public class LoggingReceiversHub : Hub<IReceiversMethodsServerCanCall>, IServerMethodsReceiversCanCall
	{
		private readonly IServerDiagnosticReceiver diagnosticsServer;

		public LoggingReceiversHub(IServerDiagnosticReceiver diagnosticsServer)
		{
			this.diagnosticsServer = diagnosticsServer;
		}
	}
}
