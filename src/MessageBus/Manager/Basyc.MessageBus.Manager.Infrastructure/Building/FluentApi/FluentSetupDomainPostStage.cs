using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupDomainPostStage : BuilderStageBase
{
	private readonly InProgressDomainRegistration inProgressDomain;

	public FluentSetupDomainPostStage(IServiceCollection services, InProgressDomainRegistration inProgressDomain) : base(services)
	{
		this.inProgressDomain = inProgressDomain;
	}

	public FluentSetupMessageStage AddMessage(string messageDisplayName, RequestType messageType = RequestType.Generic)
	{
		return new FluentSetupDomainStage(services, inProgressDomain).AddMessage(messageDisplayName, messageType);
	}

	public FluentSetupDomainStage AddDomain(string domainName)
	{
		return new RegisterMessagesFromFluentApiStage(services).AddDomain(domainName);
	}
}