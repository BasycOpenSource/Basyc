using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class RegisterMessagesFromFluentApiStage : BuilderStageBase
{
	public RegisterMessagesFromFluentApiStage(IServiceCollection services) : base(services)
	{
	}

	public FluentSetupGroupStage InGroup(string domainName)
	{
		var newGroup = new InProgressGroupRegistration();
		newGroup.DomainName = domainName;
		services.Configure<FluentApiDomainInfoProviderOptions>(x => x.GroupRegistrations.Add(newGroup));
		return new FluentSetupGroupStage(services, newGroup);
	}
}
