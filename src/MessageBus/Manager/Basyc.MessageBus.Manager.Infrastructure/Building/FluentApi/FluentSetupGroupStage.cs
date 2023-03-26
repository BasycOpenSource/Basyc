using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupGroupStage : BuilderStageBase
{
	private readonly FluentApiGroupRegistration fluentApiGroup;

	public FluentSetupGroupStage(IServiceCollection services, FluentApiGroupRegistration fluentApiGroup) : base(services)
	{
		this.fluentApiGroup = fluentApiGroup;
	}

	public FluentSetupMessageStage AddMessage(string messageDisplayName)
	{
		var newMessage = new FluentApiMessageRegistration
		{
			MessageDisplayName = messageDisplayName,
			MessageType = MessageType.Generic
		};
		fluentApiGroup.InProgressMessages.Add(newMessage);
		return new FluentSetupMessageStage(services, newMessage, fluentApiGroup);
	}
}
