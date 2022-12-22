using Basyc.Diagnostics.Producing.Shared;
using Basyc.Diagnostics.Producing.Shared.Listening;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceProviderDiagnosticsProductingExtensions
{
    public static async Task StartBasycDiagnosticExporters(this IServiceProvider serviceProvider)
    {
        var producer = serviceProvider.GetRequiredService<IDiagnosticsExporter>();
        var listenerManager = serviceProvider.GetRequiredService<DiagnosticListenerManager>();
        await listenerManager.Start();
        await producer.StartAsync();
    }
}
