using Basyc.DependencyInjection;
using Basyc.Diagnostics.Producing.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.MessageBus.Broker.NetMQ.Building
{
	public class SelectDiagnosticStage : BuilderStageBase
	{
		public SelectDiagnosticStage(IServiceCollection services) : base(services)
		{
		}

		public void NoDiagnostics()
		{
			services.TryAddSingleton<IDiagnosticsExporter, NullDiagnosticsExporter>();
		}
	}
}
