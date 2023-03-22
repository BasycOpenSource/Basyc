using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

	private FluentSetupDomainPostStage HandeledBy(RequestHandlerDelegate handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy<TReturn>(Func<RequestInput, TReturn> handler)
	{
		object? wrapperHandler(MessageRequest result, ILogger logger)
		{
			//requestResult.Start();
			var returnObject = handler.Invoke(result.Request);
			return returnObject;
		}
		inProgressMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<TMessage, object> handlerWithTReturn)
	{
		object? wrapperHandler(MessageRequest result, ILogger logger)
		{
			var message = binder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, inProgressMessage.ResponseRunTimeType!);
			return returnObject!;
		}
		inProgressMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandeledBy<TReturn>(Func<TMessage, TReturn> handlerWithTReturn)
		where TReturn : class
	{
		TReturn wrapperHandler(MessageRequest result, ILogger logger)
		{
			var message = binder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, inProgressMessage.ResponseRunTimeType!);
			return returnObject!;
		}
		inProgressMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}
}
