using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.DomainDrivenDesign.DependencyInjection;

public class InfrastructureBuilderBase<TParentBuilder> : DependencyBuilderBase<TParentBuilder>
{
    public InfrastructureBuilderBase(IServiceCollection service, TParentBuilder parentBuilder)
        : base(service, parentBuilder)
    {
    }
}
