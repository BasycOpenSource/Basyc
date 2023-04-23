using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Diagnostics;

public class SetupDiagnosticsStage : BuilderStageBase
{
    public SetupDiagnosticsStage(IServiceCollection services) : base(services)
    {
    }
}
