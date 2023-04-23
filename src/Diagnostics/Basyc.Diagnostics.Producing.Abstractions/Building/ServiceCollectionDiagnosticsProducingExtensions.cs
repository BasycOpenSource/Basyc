using Basyc.Diagnostics.Producing.Shared.Building;
using Basyc.Diagnostics.Producing.Shared.Listening;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionDiagnosticsProducingExtensions
{
    public static SetupDefaultServiceStage AddBasycDiagnosticsExporting(this IServiceCollection services)
    {
        services.AddSingleton<DiagnosticListenerManager>();
        return new SetupDefaultServiceStage(services);
    }
}
