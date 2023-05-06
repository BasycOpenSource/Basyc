using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.DomainDrivenDesign.DependencyInjection;

public class InfrastructureBuilderBase<TParentBuilder> : DependencyBuilderBase<TParentBuilder>
{
    public InfrastructureBuilderBase(IServiceCollection service, TParentBuilder parentBuilder)
        : base(service, parentBuilder)
    {
    }
}
