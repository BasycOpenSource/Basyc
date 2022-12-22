using Basyc.Diagnostics.Producing.Shared.Building;
using Basyc.Diagnostics.Producing.Shared.Listening;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionDiagnosticsProducingExtensions
{
    public static SetupDefaultServiceStage AddBasycDiagnosticExporting(this IServiceCollection services)
    {
        services.AddSingleton<DiagnosticListenerManager>();
        return new SetupDefaultServiceStage(services);
    }
}
