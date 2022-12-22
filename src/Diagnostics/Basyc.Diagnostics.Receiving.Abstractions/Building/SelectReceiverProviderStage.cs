using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Diagnostics.Receiving.Abstractions.Building;

public class SelectReceiverProviderStage : BuilderStageBase
{
	public SelectReceiverProviderStage(IServiceCollection services) : base(services)
	{
	}
}