using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class SelectHandlerStage : BuilderStageBase
{
	private readonly InterfaceRegistration interfaceRegistration;

	public SelectHandlerStage(IServiceCollection services, InterfaceRegistration interfaceRegistration) : base(services)
	{
		this.interfaceRegistration = interfaceRegistration;
	}

	public void HandledByDefaultHandler()
	{
		interfaceRegistration.RequestHandlerUniqueName = InterfaceRegistration.DefaultRequestHandlerUniqueName;
	}

	public void HandledBy(string handlerUniqueName)
	{
		interfaceRegistration.RequestHandlerUniqueName = handlerUniqueName;
	}
}
