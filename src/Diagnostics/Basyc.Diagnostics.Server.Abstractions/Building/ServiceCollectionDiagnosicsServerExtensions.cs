using Basyc.Diagnostics.Server.Abstractions;
using Basyc.Diagnostics.Server.Abstractions.Building;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionDiagnosicsServerExtensions
{
    public static SelectDiagnosticsServerStage AddBasycDiagnosticsServer(this IServiceCollection services)
    {
        services.AddSingleton<DiagnosticServer>();
        services.TryAddSingleton<InMemoryServerDiagnosticReceiver>();

        return new SelectDiagnosticsServerStage(services);
    }
}
