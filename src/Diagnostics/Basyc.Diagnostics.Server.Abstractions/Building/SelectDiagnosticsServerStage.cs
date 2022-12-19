using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Diagnostics.Server.Abstractions.Building
{
    public class SelectDiagnosticsServerStage : BuilderStageBase
    {
        public SelectDiagnosticsServerStage(IServiceCollection services) : base(services)
        {
        }
    }
}