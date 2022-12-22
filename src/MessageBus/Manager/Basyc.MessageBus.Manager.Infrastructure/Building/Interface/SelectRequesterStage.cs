using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class SelectRequesterStage : BuilderStageBase
{
	private readonly InterfaceRegistration interfaceRegistration;

	public SelectRequesterStage(IServiceCollection services, InterfaceRegistration interfaceRegistration) : base(services)
	{
		this.interfaceRegistration = interfaceRegistration;
	}

	public void HandeledByDefault()
	{
		interfaceRegistration.RequesterUniqueName = InterfaceRegistration.DefaultRequester;
	}

	public void HandeledBy(string requesterUniqueName)
	{
		interfaceRegistration.RequesterUniqueName = requesterUniqueName;
	}
}