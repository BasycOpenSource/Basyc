using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Client.Building;

public class BusClientSetupProviderStage : BuilderStageBase
{
	public BusClientSetupProviderStage(IServiceCollection services) : base(services)
	{
	}
}
