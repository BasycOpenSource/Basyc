using Basyc.Diagnostics.Server.Abstractions;
using Basyc.Diagnostics.Server.Abstractions.Building;
using Basyc.Diagnostics.SignalR.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SelectDiagnosticsServerStageSignalRExtensions
{
    /// <summary>
    ///     Call <see cref="WepApplicationDiagnosticsSignalRExtensions.MapBasycSignalRDiagnosticsServer(WebApplication, string, string)" /> to start.
    /// </summary>
    public static void SelectSignalRPusher(this SelectDiagnosticsServerStage parent)
    {
        parent.Services.AddSignalR();
        parent.Services.TryAddSingleton<InMemoryServerDiagnosticReceiver>();
        parent.Services.AddSingleton<IServerDiagnosticReceiver, InMemoryServerDiagnosticReceiver>(x =>
            x.GetRequiredService<InMemoryServerDiagnosticReceiver>());
        parent.Services.AddSingleton<IServerDiagnosticPusher, SignalRServerDiagnosticPusher>();
    }
}
