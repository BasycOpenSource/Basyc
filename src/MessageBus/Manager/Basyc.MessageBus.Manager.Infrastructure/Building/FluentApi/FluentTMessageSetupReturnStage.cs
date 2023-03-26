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
	private readonly FluentApiGroupRegistration fluentApiGroup;
	private readonly FluentApiMessageRegistration fluentApiMessage;

	public FluentTMessageSetupReturnStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage,
		FluentApiGroupRegistration fluentApiGroup) : base(services)
	{
		this.fluentApiMessage = fluentApiMessage;
		this.fluentApiGroup = fluentApiGroup;
		binder = new RequestToTypeBinder<TMessage>();
	}

	private FluentSetupDomainPostStage HandeledBy(RequestHandlerDelegate handler)
	{
		fluentApiMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}

	public FluentSetupDomainPostStage HandeledBy<TReturn>(Func<RequestInput, TReturn> handler)
	{
		object? wrapperHandler(MessageRequest result, ILogger logger)
		{
			//requestResult.Start();
			var returnObject = handler.Invoke(result.Request);
			return returnObject;
		}
		fluentApiMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<TMessage, object> handlerWithTReturn)
	{
		object? wrapperHandler(MessageRequest result, ILogger logger)
		{
			var message = binder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, fluentApiMessage.ResponseRunTimeType!);
			return returnObject!;
		}
		fluentApiMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}

	public FluentSetupDomainPostStage HandeledBy<TReturn>(Func<TMessage, TReturn> handlerWithTReturn)
		where TReturn : class
	{
		TReturn wrapperHandler(MessageRequest result, ILogger logger)
		{
			var message = binder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, fluentApiMessage.ResponseRunTimeType!);
			return returnObject!;
		}
		fluentApiMessage.RequestHandler = wrapperHandler;
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}
}
