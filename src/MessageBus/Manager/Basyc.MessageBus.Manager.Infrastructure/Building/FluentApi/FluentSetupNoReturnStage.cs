using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupNoReturnStage : BuilderStageBase
{
	private readonly InProgressDomainRegistration inProgressDomain;
	private readonly InProgressMessageRegistration inProgressMessage;

	public FluentSetupNoReturnStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage, InProgressDomainRegistration inProgressDomain) :
		base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressDomain = inProgressDomain;
	}

	public FluentSetupDomainPostStage HandledBy(Action<RequestContext> handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressDomain);
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
		return new FluentSetupDomainPostStage(services, inProgressDomain);
	}
}
