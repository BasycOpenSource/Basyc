using Basyc.DependencyInjection;
using Basyc.Diagnostics.Shared.Durations;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Diagnostics.Producing.Shared.Building;

public class SetupDefaultServiceStage : BuilderStageBase
{
    public SetupDefaultServiceStage(IServiceCollection services) : base(services)
    {
    }

    public SetupProducersStage SetDefaultIdentity(string serviceName)
    {
        ServiceIdentity serviceIdentity = new ServiceIdentity(serviceName);
        ServiceIdentity.ApplicationWideIdentity = serviceIdentity;
        return new SetupProducersStage(services);
    }
}
