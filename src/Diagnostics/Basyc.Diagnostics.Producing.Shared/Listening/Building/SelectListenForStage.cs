using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Diagnostics.Producing.Shared.Listening.Building
{
    public class SelectListenForStage : BuilderStageBase
    {
        public SelectListenForStage(IServiceCollection services) : base(services)
        {
        }
    }
}