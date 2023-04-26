using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class RegisterMessagesFromAssemblyStage : BuilderStageBase
{
    public RegisterMessagesFromAssemblyStage(IServiceCollection services, string groupName, params Assembly[] assembliesToScan) : base(services)
    {
        AssembliesToScan = assembliesToScan;
        GroupName = groupName;
    }

    public IReadOnlyCollection<Assembly> AssembliesToScan { get; init; }

    public string GroupName { get; init; }
}
