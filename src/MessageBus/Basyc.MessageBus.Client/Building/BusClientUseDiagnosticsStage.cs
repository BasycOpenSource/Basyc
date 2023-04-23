using Basyc.DependencyInjection;
using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Client.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.MessageBus.Client.Building;

public class BusClientUseDiagnosticsStage : BuilderStageBase
{
    public BusClientUseDiagnosticsStage(IServiceCollection services) : base(services)
    {
    }

    public void NoDiagnostics()
    {
        services.TryAddSingleton<IDiagnosticsExporter, NullDiagnosticsExporter>();
        services.Configure<BusDiagnosticsOptions>(x =>
        {
            x.UseDiagnostics = false;
        });
    }

    public BusClientSetupDiagnosticsStage EnableDiagnostics()
    {
        services.Configure<BusDiagnosticsOptions>(x =>
        {
            x.UseDiagnostics = true;
            x.Service = ServiceIdentity.ApplicationWideIdentity;
        });
        return new BusClientSetupDiagnosticsStage(services);
    }
}
