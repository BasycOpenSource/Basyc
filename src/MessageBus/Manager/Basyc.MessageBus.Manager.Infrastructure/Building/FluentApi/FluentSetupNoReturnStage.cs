using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupNoReturnStage : BuilderStageBase
{
	private readonly InProgressGroupRegistration inProgressGroup;
	private readonly InProgressMessageRegistration inProgressMessage;

	public FluentSetupNoReturnStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage, InProgressGroupRegistration inProgressGroup) :
		base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressGroup = inProgressGroup;
	}

	public FluentSetupDomainPostStage HandledBy(Action<RequestContext> handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandledBy(Action<Request> handler)
	{
		void ToRequestContextAction(RequestContext requestResult)
		{
			//requestResult.Start();
			handler.Invoke(requestResult.Request);
			requestResult.Complete();
		}

		inProgressMessage.RequestHandler = ToRequestContextAction;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}
}
