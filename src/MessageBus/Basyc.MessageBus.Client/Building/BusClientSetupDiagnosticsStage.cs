using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Client.Building
{
    public class BusClientSetupDiagnosticsStage : BuilderStageBase
    {
        public BusClientSetupDiagnosticsStage(IServiceCollection services) : base(services)
        {
        }
    }
}