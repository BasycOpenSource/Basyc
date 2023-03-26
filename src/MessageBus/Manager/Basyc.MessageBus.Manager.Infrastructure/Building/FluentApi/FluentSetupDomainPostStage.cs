using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupDomainPostStage : BuilderStageBase
{
	private readonly FluentApiGroupRegistration inProgressGroup;

	public FluentSetupDomainPostStage(IServiceCollection services, FluentApiGroupRegistration inProgressGroup) : base(services)
	{
		this.inProgressGroup = inProgressGroup;
	}

	public FluentSetupMessageStage AddMessage(string messageDisplayName)
	{
		return new FluentSetupGroupStage(services, inProgressGroup).AddMessage(messageDisplayName);
	}

	public FluentSetupGroupStage AddDomain(string domainName)
	{
		return new RegisterMessagesFromFluentApiStage(services).AddGroup(domainName);
	}
}
