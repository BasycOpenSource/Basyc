using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class SetupGroupStage : BuilderStageBase
{
    private readonly Assembly[] assemblies;

    public SetupGroupStage(IServiceCollection services, Assembly[] assemblies) : base(services)
    {
        this.assemblies = assemblies;
    }

    public RegisterMessagesFromAssemblyStage InGroup(string groupName) => new RegisterMessagesFromAssemblyStage(services, groupName, assemblies);
}
