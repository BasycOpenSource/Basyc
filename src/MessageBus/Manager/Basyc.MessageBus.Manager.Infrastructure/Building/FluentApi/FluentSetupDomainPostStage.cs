using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupDomainPostStage : BuilderStageBase
{
	private readonly InProgressGroupRegistration inProgressGroup;

	public FluentSetupDomainPostStage(IServiceCollection services, InProgressGroupRegistration inProgressGroup) : base(services)
	{
		this.inProgressGroup = inProgressGroup;
	}

	public FluentSetupMessageStage AddMessage(string messageDisplayName, MessageType messageType = MessageType.Generic)
	{
		return new FluentSetupGroupStage(services, inProgressGroup).AddMessage(messageDisplayName, messageType);
	}

	public FluentSetupGroupStage AddDomain(string domainName)
	{
		return new RegisterMessagesFromFluentApiStage(services).InGroup(domainName);
	}
}
