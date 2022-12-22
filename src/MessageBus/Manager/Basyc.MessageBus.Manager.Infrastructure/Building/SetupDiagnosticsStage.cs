using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class SetupDiagnosticsStage : BuilderStageBase
{
    public SetupDiagnosticsStage(IServiceCollection services) : base(services)
    {
    }

    public SetupRequesterStage NoDiagnostics()
    {
        return new SetupRequesterStage(services);
    }
}
