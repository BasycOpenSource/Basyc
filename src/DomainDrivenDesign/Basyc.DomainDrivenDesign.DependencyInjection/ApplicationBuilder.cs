using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.DomainDrivenDesign.DependencyInjection;

public class ApplicationBuilder<TParentBuilder, TInfraBuilder> : DependencyBuilderBase<TParentBuilder>
    where TInfraBuilder : InfrastructureBuilderBase<ApplicationBuilder<TParentBuilder, TInfraBuilder>>
{
    private readonly TInfraBuilder infraBuilder;

    public ApplicationBuilder(IServiceCollection services, TParentBuilder parentBuilder, TInfraBuilder infraBuilder) : base(services, parentBuilder)
    {
        this.infraBuilder = infraBuilder;
    }

    public TInfraBuilder UseInfrastructure() => infraBuilder;
}
