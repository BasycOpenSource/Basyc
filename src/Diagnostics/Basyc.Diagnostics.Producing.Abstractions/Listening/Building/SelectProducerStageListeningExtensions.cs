using Basyc.Diagnostics.Producing.Shared.Building;
using Basyc.Diagnostics.Producing.Shared.Listening.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class SelectProducerStageListeningExtensions
{
	public static SelectListenForStage ListenFor(this SetupProducersStage parent)
	{
		return new SelectListenForStage(parent.services);
	}
}
