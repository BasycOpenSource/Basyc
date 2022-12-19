using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface
{
	public class SelectMessageTypeStage : BuilderStageBase
	{
		private readonly InterfaceRegistration interfaceRegistration;

		public SelectMessageTypeStage(IServiceCollection services, InterfaceRegistration interfaceRegistration) : base(services)
		{
			this.interfaceRegistration = interfaceRegistration;
		}

		public SelectRequesterStage AsEvents()
		{
			interfaceRegistration.RequestType = RequestType.Event;
			return new SelectRequesterStage(services, interfaceRegistration);
		}

		public SetupHasResponseStage AsQueries()
		{
			interfaceRegistration.RequestType = RequestType.Query;
			return new SetupHasResponseStage(services, interfaceRegistration);
		}

		public SetupHasResponseStage AsCommands()
		{
			interfaceRegistration.RequestType = RequestType.Command;
			return new SetupHasResponseStage(services, interfaceRegistration);
		}
	}
}
