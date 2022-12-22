using Basyc.DependencyInjection;
using Basyc.Diagnostics.Producing.Shared;
using Basyc.Diagnostics.Shared.Durations;
using Basyc.MessageBus.Client.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.MessageBus.Client.Building;

public class BusClientUseDiagnosticsStage : BuilderStageBase
{
    public BusClientUseDiagnosticsStage(IServiceCollection services) : base(services)
    {
    }

#pragma warning disable CA1822 // Mark members as static
    public void NoDiagnostics()
#pragma warning restore CA1822 // Mark members as static
    {
        services.TryAddSingleton<IDiagnosticsExporter, NullDiagnosticsExporter>();
        services.Configure<BusDiagnosticsOptions>(x =>
        {
            x.UseDiagnostics = false;
        });
    }

    public BusClientSetupDiagnosticsStage UseDiagnostics()
    {
        services.Configure<BusDiagnosticsOptions>(x =>
        {
            x.UseDiagnostics = true;
            x.Service = ServiceIdentity.ApplicationWideIdentity;
        });
        return new BusClientSetupDiagnosticsStage(services);
    }
}
