using Basyc.Diagnostics.Server.Abstractions;
using Basyc.Diagnostics.SignalR.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Basyc.Diagnostics.SignalR.Server;

public class LoggingReceiversHub : Hub<IReceiversMethodsServerCanCall>, IServerMethodsReceiversCanCall
{
    public LoggingReceiversHub(IServerDiagnosticReceiver diagnosticsServer)
    {
    }
}
