using Microsoft.Extensions.DependencyInjection;

namespace Basyc.DependencyInjection;

public class BuilderStageBase
{
    public BuilderStageBase(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; init; }
}
