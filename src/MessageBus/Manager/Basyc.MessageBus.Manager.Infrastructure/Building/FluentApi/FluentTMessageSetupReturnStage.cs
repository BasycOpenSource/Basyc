using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentTMessageSetupReturnStage<TMessage> : BuilderStageBase
{
	private readonly RequestToTypeBinder<TMessage> binder;
	private readonly InProgressGroupRegistration inProgressGroup;
	private readonly InProgressMessageRegistration inProgressMessage;

	public FluentTMessageSetupReturnStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage,
		InProgressGroupRegistration inProgressGroup) : base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressGroup = inProgressGroup;
		binder = new RequestToTypeBinder<TMessage>();
	}

	public FluentSetupDomainPostStage HandeledBy(Action<RequestContext> handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy<TReturn>(Func<Request, TReturn> handler)
	{
		Action<RequestContext> handlerWrapper = requestResult =>
		{
			//requestResult.Start();
			var returnObject = handler.Invoke(requestResult.Request);
			requestResult.Complete(returnObject);
		};
		inProgressMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<TMessage, object> handlerWithTReturn)
	{
		Action<RequestContext> wrapperHandler = result =>
		{
			var message = binder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, inProgressMessage.ResponseRunTimeType!);
			result.Complete(returnObject!);
		};
		inProgressMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy<TReturn>(Func<TMessage, TReturn> handlerWithTReturn)
		where TReturn : class
	{
		Action<RequestContext> wrapperHandler = result =>
		{
			var message = binder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, inProgressMessage.ResponseRunTimeType!);
			result.Complete(returnObject!);
		};
		inProgressMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}
}
