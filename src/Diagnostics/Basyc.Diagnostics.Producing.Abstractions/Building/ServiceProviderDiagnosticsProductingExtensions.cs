using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Producing.Shared.Listening;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderDiagnosticsProductingExtensions
{
    public static async Task StartBasycDiagnosticExporters(this IServiceProvider serviceProvider)
    {
        var exporters = serviceProvider.GetServices<IDiagnosticsExporter>();
        var listenerManager = serviceProvider.GetRequiredService<DiagnosticListenerManager>();
        await listenerManager.Start();
        foreach (var diagnosticsExporter in exporters)
            await diagnosticsExporter.StartAsync();
    }
}
