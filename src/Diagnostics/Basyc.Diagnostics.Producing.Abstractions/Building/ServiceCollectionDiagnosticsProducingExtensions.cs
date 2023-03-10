using Basyc.Diagnostics.Producing.Shared.Listening;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Diagnostics.Producing.Shared.Building;

public static class ServiceCollectionDiagnosticsProducingExtensions
{
	public static SetupDefaultServiceStage AddBasycDiagnosticExporting(this IServiceCollection services)
	{
		services.AddSingleton<DiagnosticListenerManager>();
		return new SetupDefaultServiceStage(services);
	}
}
