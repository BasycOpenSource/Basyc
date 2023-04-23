using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.DependencyInjection;

public abstract class DependencyBuilderBase<TParentBuilder>
{
    public readonly IServiceCollection services;
    private readonly TParentBuilder parentBuilder;

    public DependencyBuilderBase(IServiceCollection services, TParentBuilder parentBuilder)
    {
        this.services = services;
        this.parentBuilder = parentBuilder;
    }

    /// <summary>
    /// Allows continuing configuring previous builder
    /// </summary>
    /// <returns></returns>
    public TParentBuilder Back() => parentBuilder;
}

public abstract class DependencyBuilderBase
{
    public readonly IServiceCollection services;

    public DependencyBuilderBase(IServiceCollection services)
    {
        this.services = services;
    }
}
