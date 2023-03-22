using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

	private FluentSetupDomainPostStage HandeledBy(RequestHandlerDelegate handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandledBy<TReturn>(Func<RequestInput, ILogger, TReturn> handler)
		where TReturn : class
	{
		object? handlerWrapper(MessageRequest requestResult, ILogger logger)
		{
			var returnObject = handler.Invoke(requestResult.Request, logger);
			returnObject.ThrowIfNull();
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, requestResult.Request.MessageInfo.ResponseType!);
			//requestResult.Complete(returnObject);
			return returnObject;
		}
		inProgressMessage.RequestHandler = handlerWrapper;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}
}
