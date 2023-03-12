using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupTypeOfReturnStage : BuilderStageBase
{
	private readonly InProgressGroupRegistration inProgressGroup;
	private readonly InProgressMessageRegistration inProgressMessage;

	public FluentSetupTypeOfReturnStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage,
		InProgressGroupRegistration inProgressGroup) : base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressGroup = inProgressGroup;
	}

	public FluentSetupDomainPostStage HandeledBy(Action<MessageRequest> handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy<TReturn>(Func<RequestInput, TReturn> handler)
		where TReturn : class
	{
		Action<MessageRequest> handlerWrapper = requestResult =>
		{
			var returnObject = handler.Invoke(requestResult.Request);
			returnObject.ThrowIfNull();
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, requestResult.Request.MessageInfo.ResponseType!);
			requestResult.Complete(returnObject);
		};
		inProgressMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}
}
