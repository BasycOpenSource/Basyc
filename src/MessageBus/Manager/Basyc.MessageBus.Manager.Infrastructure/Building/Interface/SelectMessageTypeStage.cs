using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class SelectMessageTypeStage : BuilderStageBase
{
    private readonly InterfaceRegistration interfaceRegistration;

    public SelectMessageTypeStage(IServiceCollection services, InterfaceRegistration interfaceRegistration) : base(services)
    {
        this.interfaceRegistration = interfaceRegistration;
    }

    public SelectHandlerStage AsEvents()
    {
        interfaceRegistration.RequestType = MessageType.Event;
        return new SelectHandlerStage(services, interfaceRegistration);
    }

    public SetupHasResponseStage AsQueries()
    {
        interfaceRegistration.RequestType = MessageType.Query;
        return new SetupHasResponseStage(services, interfaceRegistration);
    }

    public SetupHasResponseStage AsCommands()
    {
        interfaceRegistration.RequestType = MessageType.Command;
        return new SetupHasResponseStage(services, interfaceRegistration);
    }
}
