using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupDomainPostStage : BuilderStageBase
{
	private readonly FluentApiGroupRegistration fluentApiGroup;

	public FluentSetupDomainPostStage(IServiceCollection services, FluentApiGroupRegistration fluentApiGroup) : base(services)
	{
		this.fluentApiGroup = fluentApiGroup;
	}

	public FluentSetupMessageStage AddMessage(string messageDisplayName, MessageType messageType = MessageType.Generic)
	{
		return new FluentSetupGroupStage(services, fluentApiGroup).AddMessage(messageDisplayName, messageType);
	}

	public FluentSetupGroupStage AddDomain(string domainName)
	{
		return new RegisterMessagesFromFluentApiStage(services).AddGroup(domainName);
	}
}
