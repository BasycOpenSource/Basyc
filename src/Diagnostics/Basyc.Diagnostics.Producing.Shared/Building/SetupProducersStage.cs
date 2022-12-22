using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.Diagnostics.Producing.Shared.Building;

public class SetupProducersStage : BuilderStageBase
{
	public SetupProducersStage(IServiceCollection services) : base(services)
	{
	}

	public SetupProducersStage SelectInMemoryExporter()
	{
		services.TryAddSingleton<InMemoryDiagnosticsExporter>();
		services.AddSingleton<IDiagnosticsExporter, InMemoryDiagnosticsExporter>(x => x.GetRequiredService<InMemoryDiagnosticsExporter>());
		return new SetupProducersStage(services);
	}

	public SetupProducersStage SelectNullExporter()
	{
		services.AddSingleton<IDiagnosticsExporter, NullDiagnosticsExporter>();
		return new SetupProducersStage(services);
	}
}