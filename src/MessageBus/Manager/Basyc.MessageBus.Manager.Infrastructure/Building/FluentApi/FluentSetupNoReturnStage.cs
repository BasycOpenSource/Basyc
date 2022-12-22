using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupNoReturnStage : BuilderStageBase
{
	private readonly InProgressMessageRegistration inProgressMessage;
	private readonly InProgressDomainRegistration inProgressDomain;

	public FluentSetupNoReturnStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage, InProgressDomainRegistration inProgressDomain) : base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressDomain = inProgressDomain;
	}

	public FluentSetupDomainPostStage HandeledBy(Action<RequestContext> handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressDomain);
	}

	public FluentSetupDomainPostStage HandeledBy(Action<Request> handler)
	{
		Action<RequestContext> handlerWrapper = (requestResult) =>
		{
			//requestResult.Start();
			handler.Invoke(requestResult.Request);
			requestResult.Complete();
		};
		inProgressMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, inProgressDomain);
	}
}