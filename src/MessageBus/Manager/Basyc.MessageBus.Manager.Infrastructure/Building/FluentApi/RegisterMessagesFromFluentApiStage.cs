using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi
{
	public class RegisterMessagesFromFluentApiStage : BuilderStageBase
	{
		public RegisterMessagesFromFluentApiStage(IServiceCollection services) : base(services)
		{
		}

		public FluentSetupDomainStage AddDomain(string domainName)
		{
			InProgressDomainRegistration newDomain = new InProgressDomainRegistration();
			newDomain.DomainName = domainName;
			services.Configure<FluentApiDomainInfoProviderOptions>(x => x.DomainRegistrations.Add(newDomain));
			return new FluentSetupDomainStage(services, newDomain);
		}
	}
}
