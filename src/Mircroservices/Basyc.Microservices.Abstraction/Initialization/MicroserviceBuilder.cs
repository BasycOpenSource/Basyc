using Basyc.DependencyInjection;
using Basyc.MicroService.Abstraction.Initialization;

namespace Microsoft.Extensions.DependencyInjection;

public class MicroserviceBuilder<TParentBuilder> : DependencyBuilderBase<TParentBuilder>
{
    public MicroserviceBuilder(IServiceCollection services, TParentBuilder parentBuilder) : base(services, parentBuilder)
    {
    }

    public IMicroserviceProvider? MicroserviceProvider { get; private set; }

    public MicroserviceBuilder<TParentBuilder> AddProvider(IMicroserviceProvider provider)
    {
        MicroserviceProvider = provider;
        return this;
    }
}
