using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;

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

	public FluentSetupDomainPostStage HandeledBy(Action<MessageRequest> handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<RequestInput, TReturn> handler)
	{
		Action<MessageRequest> handlerWrapper = (requestResult) =>
		{
			var returnObject = handler.Invoke(requestResult.Request);
			requestResult.Complete(returnObject);
		};
		inProgressMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<TMessage, TReturn> handlerWithTReturn)
	{
		Action<MessageRequest> wrapperHandler = (result) =>
		{
			var message = messageBinder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			result.Complete(returnObject!);
		};
		inProgressMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}
}
