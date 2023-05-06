using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class SetupDisplayNameStage : BuilderStageBase
{
    private readonly InterfaceRegistration interfaceRegistration;

    public SetupDisplayNameStage(IServiceCollection services, InterfaceRegistration interfaceRegistration) : base(services)
    {
        this.interfaceRegistration = interfaceRegistration;
    }

    public SelectMessageTypeStage UseTypeNameAsDisplayName()
    {
        interfaceRegistration.DisplayNameFormatter = x => x.Name;
        return new SelectMessageTypeStage(Services, interfaceRegistration);
    }

    public SelectMessageTypeStage SetDisplayName(Func<Type, string> formatter)
    {
        interfaceRegistration.DisplayNameFormatter = formatter;
        return new SelectMessageTypeStage(Services, interfaceRegistration);
    }
}
