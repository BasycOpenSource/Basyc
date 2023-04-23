using Basyc.Diagnostics.Server.Abstractions;
using Basyc.Diagnostics.SignalR.Shared;
using Basyc.Diagnostics.SignalR.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace Basyc.Diagnostics.SignalR.Server;

public class LoggingProducersHub : Hub<IProducersMethodsServerCanCall>, IServerMethodsProducersCanCall
{
    private readonly InMemoryServerDiagnosticReceiver diagnosticsReceiver;

    public LoggingProducersHub(InMemoryServerDiagnosticReceiver diagnosticsReceiver)
    {
        this.diagnosticsReceiver = diagnosticsReceiver;
    }

    public Task ReceiveChangesFromProducer(ChangesSignalRDto changesDto)
    {
        diagnosticsReceiver.ReceiveChangesFromProducer(ChangesSignalRDto.FromDto(changesDto));
        return Task.CompletedTask;
    }
}
