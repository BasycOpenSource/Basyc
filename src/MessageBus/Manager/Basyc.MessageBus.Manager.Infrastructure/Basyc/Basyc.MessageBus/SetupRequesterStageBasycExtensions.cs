using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;

public static class SetupRequesterStageBasycExtensions
{
	public static SetupTypeFormattingStage UseBasycMessageBusHandler(this SetupRequesterStage parent)
	{
		parent.services.AddSingleton<IRequestHandler, BasycTypedMessageBusRequestHandler>();
		return new SetupTypeFormattingStage(parent.services);
	}
}
