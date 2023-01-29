using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupTypeOfReturnStage : BuilderStageBase
{
	private readonly InProgressDomainRegistration inProgressDomain;
	private readonly InProgressMessageRegistration inProgressMessage;

	public FluentSetupTypeOfReturnStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage,
		InProgressDomainRegistration inProgressDomain) : base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressDomain = inProgressDomain;
	}

	public FluentSetupDomainPostStage HandeledBy(Action<RequestContext> handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressDomain);
	}

	public FluentSetupDomainPostStage HandeledBy<TReturn>(Func<Request, TReturn> handler)
		where TReturn : class
	{
		Action<RequestContext> handlerWrapper = requestResult =>
		{
			var returnObject = handler.Invoke(requestResult.Request);
			returnObject.ThrowIfNull();
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, requestResult.Request.RequestInfo.ResponseType!);
			requestResult.Complete(returnObject);
		};
		inProgressMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, inProgressDomain);
	}
}
