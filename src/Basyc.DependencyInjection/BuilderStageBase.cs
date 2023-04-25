using Microsoft.Extensions.DependencyInjection;

namespace Basyc.DependencyInjection;

public class BuilderStageBase
{
    public BuilderStageBase(IServiceCollection services)
    {
        this.Services = services;
    }

    public IServiceCollection Services { get; init; }
}
