using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

namespace Microsoft.Extensions.DependencyInjection;

public class BusManagerApplicationBuilder : BuilderStageBase
{
    public BusManagerApplicationBuilder(IServiceCollection services) : base(services)
    {

    }

    public SetupMessagesStage RegisterMessages() => new SetupMessagesStage(services);
}
