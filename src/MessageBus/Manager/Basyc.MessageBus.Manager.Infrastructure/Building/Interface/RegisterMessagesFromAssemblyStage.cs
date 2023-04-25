using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class RegisterMessagesFromAssemblyStage : BuilderStageBase
{
    public RegisterMessagesFromAssemblyStage(IServiceCollection services, string groupName, params Assembly[] assembliesToScan) : base(services)
    {
        this.AssembliesToScan = assembliesToScan;
        this.GroupName = groupName;
    }

    public Assembly[] AssembliesToScan { get; init; }

    public string GroupName { get; init; }
}
