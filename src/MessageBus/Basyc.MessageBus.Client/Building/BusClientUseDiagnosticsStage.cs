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
        Services.TryAddSingleton<IDiagnosticsExporter, NullDiagnosticsExporter>();
        Services.Configure<BusDiagnosticsOptions>(x =>
        {
            x.UseDiagnostics = false;
        });
    }

    public BusClientSetupDiagnosticsStage EnableDiagnostics()
    {
        Services.Configure<BusDiagnosticsOptions>(x =>
        {
            x.UseDiagnostics = true;
            x.Service = ServiceIdentity.ApplicationWideIdentity;
        });
        return new BusClientSetupDiagnosticsStage(Services);
    }
}
