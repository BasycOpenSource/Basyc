using Basyc.DependencyInjection;
using Basyc.Diagnostics.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Diagnostics.Producing.Shared.Building;

public class SetupDefaultServiceStage : BuilderStageBase
{
    public SetupDefaultServiceStage(IServiceCollection services) : base(services)
    {
    }

    public SetupProducersStage SetDefaultIdentity(string serviceName)
    {
        var serviceIdentity = new ServiceIdentity(serviceName);
        ServiceIdentity.ApplicationWideIdentity = serviceIdentity;
        return new SetupProducersStage(services);
    }
}
