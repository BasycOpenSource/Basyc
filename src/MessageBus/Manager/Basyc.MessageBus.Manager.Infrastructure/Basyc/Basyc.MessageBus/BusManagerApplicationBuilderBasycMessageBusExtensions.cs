using Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class BusManagerApplicationBuilderBasycMessageBusExtensions
	{
		public static void RegisterBasycMessageBusRequester(this BusManagerApplicationBuilder parent)
		{
			parent.services.TryAddSingleton<IRequester, BasycTypedMessageBusRequester>();

		}


	}
}
