using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupNoReturnStage : BuilderStageBase
{
	private readonly InProgressGroupRegistration inProgressGroup;
	private readonly InProgressMessageRegistration inProgressMessage;

	public FluentSetupNoReturnStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage, InProgressGroupRegistration inProgressGroup) :
		base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressGroup = inProgressGroup;
	}

	private FluentSetupDomainPostStage HandledBy(RequestHandlerDelegate handler)
	{
		inProgressMessage.RequestHandler = handler;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}

	public FluentSetupDomainPostStage HandledBy(Action<RequestInput, ILogger> handler)
	{
		object? ToRequestContextAction(MessageRequest requestResult, ILogger logger)
		{
			using var act = logger.StartActivity("Invoking handler");
			handler.Invoke(requestResult.Request, logger);
			act.Stop();
			return null;
		}

		inProgressMessage.RequestHandler = ToRequestContextAction;
		return new FluentSetupDomainPostStage(services, inProgressGroup);
	}
}
