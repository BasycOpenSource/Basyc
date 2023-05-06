using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Infrastructure.Building;

namespace Microsoft.Extensions.DependencyInjection;

public class BusManagerApplicationBuilder : BuilderStageBase
{
    public BusManagerApplicationBuilder(IServiceCollection services) : base(services)
    {
    }

    public SetupMessagesStage RegisterMessages() => new(Services);
}
