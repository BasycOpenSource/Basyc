using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

namespace Microsoft.Extensions.DependencyInjection;

public static class FluentSetupMessagesStageExtensions
{
	public static RegisterMessagesFromFluentApiStage FromFluentApi(this SetupMessagesStage parent)
	{
		// var wasRegistered = parent.services.Any(x =>
		// {
		// 	if (x.ImplementationType is null)
		// 		return false;
		//
		// 	return x.ImplementationType == typeof(FluentApiDomainInfoProvider);
		// });
		// if (wasRegistered is false)
		parent.services.AddSingleton<IMessageInfoProvider, FluentApiMessageInfoProvider>();
		return new RegisterMessagesFromFluentApiStage(parent.services);
	}
}
