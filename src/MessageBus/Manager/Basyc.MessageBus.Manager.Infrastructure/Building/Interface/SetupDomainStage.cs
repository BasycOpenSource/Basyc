using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface
{
	public class SetupDomainStage : BuilderStageBase
	{
		private readonly Assembly[] assemblies;

		public SetupDomainStage(IServiceCollection services, Assembly[] assemblies) : base(services)
		{
			this.assemblies = assemblies;
		}

		public RegisterMessagesFromAssemblyStage AsDomain(string domainName)
		{
			return new(services, domainName, assemblies);
		}
	}
}
