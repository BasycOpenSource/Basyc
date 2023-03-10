using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class RegisterMessagesFromAssemblyStage : BuilderStageBase
{
	public readonly Assembly[] assembliesToScan;
	public readonly string groupName;

	public RegisterMessagesFromAssemblyStage(IServiceCollection services, string groupName, params Assembly[] assembliesToScan) : base(services)
	{
		this.assembliesToScan = assembliesToScan;
		this.groupName = groupName;
	}
}
