using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class SetupRequesterStage : BuilderStageBase
{
    public SetupRequesterStage(IServiceCollection services) : base(services)
    {
    }

    public SetupTypeFormattingStage UseHandler<TRequestHandler>()
        where TRequestHandler : class, IRequestHandler
    {
        Services.TryAddSingleton<IRequestHandler, TRequestHandler>();
        return new SetupTypeFormattingStage(Services);
    }
}
