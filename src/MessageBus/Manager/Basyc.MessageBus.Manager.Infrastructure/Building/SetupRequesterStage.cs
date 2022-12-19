using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.MessageBus.Manager.Infrastructure.Building
{
	public class SetupRequesterStage : BuilderStageBase
	{
		public SetupRequesterStage(IServiceCollection services) : base(services)
		{
		}

		public SetupTypeFormattingStage UseBasycTypedMessageBusRequester()
		{
			services.TryAddSingleton<IRequester, BasycTypedMessageBusRequester>();
			return new SetupTypeFormattingStage(services);
		}

	}
}
