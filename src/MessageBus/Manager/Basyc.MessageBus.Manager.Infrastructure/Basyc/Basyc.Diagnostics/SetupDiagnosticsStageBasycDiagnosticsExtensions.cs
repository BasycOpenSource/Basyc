using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.Diagnostics;
using Basyc.MessageBus.Manager.Infrastructure.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class SetupDiagnosticsStageBasycDiagnosticsExtensions
{
	public static SetupBasycDiagnosticsReceiverMapper UseBasycDiagnosticsReceivers(this SetupDiagnosticsStage parent)
	{
		parent.services.AddSingleton<IRequestDiagnosticsSource, BasycDiagnosticsReceiversRequestDiagnosticsSource>();
		return new SetupBasycDiagnosticsReceiverMapper(parent.services);
	}
}