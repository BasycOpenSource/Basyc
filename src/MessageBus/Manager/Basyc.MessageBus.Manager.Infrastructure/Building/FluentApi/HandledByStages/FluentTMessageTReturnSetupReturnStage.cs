using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.HandledByStages;

public class FluentTMessageTReturnSetupReturnStage<TMessage, TReturn> : BuilderStageBase
{
	private static readonly RequestToTypeBinder<TMessage> messageBinder;
	private readonly FluentApiGroupRegistration fluentApiGroup;
	private readonly FluentApiMessageRegistration fluentApiMessage;

	static FluentTMessageTReturnSetupReturnStage()
	{
		messageBinder = new RequestToTypeBinder<TMessage>();
	}

	public FluentTMessageTReturnSetupReturnStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage, FluentApiGroupRegistration fluentApiGroup) :
		base(services)
	{
		this.fluentApiMessage = fluentApiMessage;
		this.fluentApiGroup = fluentApiGroup;
	}

	//private FluentSetupDomainPostStage HandeledBy(RequestHandlerDelegate handler)
	//{
	//	fluentApiMessage.RequestHandler = handler;
	//	return new FluentSetupDomainPostStage(services, fluentApiGroup);
	//}

	public FluentSetupDomainPostStage HandeledBy(Func<RequestInput, TReturn> handler)
	{
		Task<object?> handlerWrapper(MessageRequest requestResult, ILogger logger)
		{
			var returnObject = handler.Invoke(requestResult.RequestInput);
			return Task.FromResult<object?>(returnObject);
		}

		//fluentApiMessage.RequestHandler = handlerWrapper;
		ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<TMessage, TReturn> handlerWithTReturn)
	{
		Task<object?> handlerWrapper(MessageRequest result, ILogger logger)
		{
			var message = messageBinder.CreateMessage(result.RequestInput);
			var returnObject = handlerWithTReturn.Invoke(message);
			return Task.FromResult<object?>(returnObject);
		}

		//fluentApiMessage.RequestHandler = handlerWrapper;
		ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);

		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}
}
