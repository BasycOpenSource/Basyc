using Basyc.MessageBus.Manager.Infrastructure.Building;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class SetupMessagesStageInterfaceExtensions
{
	public static SetupGroupStage FromAssemblyScan(this SetupMessagesStage parent, params Assembly[] assembliesToScan)
	{
		return new SetupGroupStage(parent.services, assembliesToScan);
	}
}
