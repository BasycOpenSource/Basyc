using Microsoft.Extensions.DependencyInjection;

namespace Basyc.DependencyInjection;

public class BuilderStageBase
{
	public readonly IServiceCollection services;

	public BuilderStageBase(IServiceCollection services)
	{
		this.services = services;
	}
}
