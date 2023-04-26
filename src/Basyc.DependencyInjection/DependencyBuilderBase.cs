using Microsoft.Extensions.DependencyInjection;

namespace Basyc.DependencyInjection;

#pragma warning disable SA1402
public abstract class DependencyBuilderBase<TParentBuilder>
{
    private readonly TParentBuilder parentBuilder;

    protected DependencyBuilderBase(IServiceCollection services, TParentBuilder parentBuilder)
    {
        Services = services;
        this.parentBuilder = parentBuilder;
    }

    public IServiceCollection Services { get; init; }

    /// <summary>
    /// Allows continuing configuring previous builder.
    /// </summary>
    public TParentBuilder Back() => parentBuilder;
}

public abstract class DependencyBuilderBase
{
    protected DependencyBuilderBase(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; init; }
}
