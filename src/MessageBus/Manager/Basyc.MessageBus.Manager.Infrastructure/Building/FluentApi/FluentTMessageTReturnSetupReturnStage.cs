using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentTMessageTReturnSetupReturnStage<TMessage, TReturn> : BuilderStageBase
{
	private readonly FluentApiMessageRegistration fluentApiMessage;
	private readonly FluentApiGroupRegistration fluentApiGroup;
	private readonly RequestToTypeBinder<TMessage> messageBinder;

	public FluentTMessageTReturnSetupReturnStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage, FluentApiGroupRegistration fluentApiGroup) : base(services)
	{
		this.fluentApiMessage = fluentApiMessage;
		this.fluentApiGroup = fluentApiGroup;

		messageBinder = new RequestToTypeBinder<TMessage>();
	}

	private FluentSetupDomainPostStage HandeledBy(RequestHandlerDelegate handler)
	{
		fluentApiMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<RequestInput, TReturn> handler)
	{
		object? handlerWrapper(MessageRequest requestResult, ILogger logger)
		{
			var returnObject = handler.Invoke(requestResult.Request);
			return returnObject;
		}
		fluentApiMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}

	public FluentSetupDomainPostStage HandeledBy(Func<TMessage, TReturn> handlerWithTReturn)
	{
		object? handlerWrapper(MessageRequest result, ILogger logger)
		{
			var message = messageBinder.CreateMessage(result.Request);
			var returnObject = handlerWithTReturn.Invoke(message);
			return returnObject!;
		}
		fluentApiMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}
}
