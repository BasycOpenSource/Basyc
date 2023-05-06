using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class SetupMessagesStage : BuilderStageBase
{
    public SetupMessagesStage(IServiceCollection services) : base(services)
    {
    }
}
