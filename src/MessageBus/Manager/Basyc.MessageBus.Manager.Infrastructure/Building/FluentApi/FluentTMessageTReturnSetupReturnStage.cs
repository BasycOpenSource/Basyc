using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentTMessageTReturnSetupReturnStage<TMessage, TReturn> : BuilderStageBase
{
	private readonly InProgressMessageRegistration inProgressMessage;
	private readonly InProgressGroupRegistration inProgressGroup;
	private readonly RequestToTypeBinder<TMessage> messageBinder;

	public FluentTMessageTReturnSetupReturnStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage, InProgressGroupRegistration inProgressGroup) : base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressGroup = inProgressGroup;

		messageBinder = new RequestToTypeBinder<TMessage>();
	}

	private FluentSetupDomainPostStage HandeledBy(RequestHandlerDelegate handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<RequestInput, TReturn> handler)
	{
		object? handlerWrapper(MessageRequest requestResult, ILogger logger)
		{
			var returnObject = handler.Invoke(requestResult.Request);
			return returnObject;
		}
		inProgressMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<TMessage, TReturn> handlerWithTReturn)
	{
		object? handlerWrapper(MessageRequest result, ILogger logger)
		{
			var message = messageBinder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			return returnObject!;
		}
		inProgressMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}
}
