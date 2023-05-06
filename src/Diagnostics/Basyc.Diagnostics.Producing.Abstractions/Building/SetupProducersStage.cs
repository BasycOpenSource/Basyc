using Basyc.DependencyInjection;
using Basyc.Diagnostics.Producing.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.Diagnostics.Producing.Shared.Building;

public class SetupProducersStage : BuilderStageBase
{
    public SetupProducersStage(IServiceCollection services) : base(services)
    {
    }

    public SetupProducersStage AddInMemoryExporter()
    {
        Services.TryAddSingleton<InMemoryDiagnosticsExporter>();
        Services.AddSingleton<IDiagnosticsExporter, InMemoryDiagnosticsExporter>(x => x.GetRequiredService<InMemoryDiagnosticsExporter>());
        return new SetupProducersStage(Services);
    }

    public SetupProducersStage AddNullExporter()
    {
        Services.AddSingleton<IDiagnosticsExporter, NullDiagnosticsExporter>();
        return new SetupProducersStage(Services);
    }
}
