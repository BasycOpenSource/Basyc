using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Diagnostics.Receiving.Abstractions.Building;

public class SetupInMemoryReceiverStage : BuilderStageBase
{
    public SetupInMemoryReceiverStage(IServiceCollection services) : base(services)
    {
    }
}
