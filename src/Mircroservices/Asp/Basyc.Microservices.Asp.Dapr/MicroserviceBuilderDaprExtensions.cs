//using Dapr.Actors.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MicroService.Asp.Dapr;

public static class MicroserviceBuilderDaprExtensions
{
    public static MicroserviceBuilder<TParentBuilder> AddDaprProvider<TParentBuilder>(this MicroserviceBuilder<TParentBuilder> builder) =>
        //var provider = new DaprMicroserviceProvider(builder.webBuilder);
        //builder.AddProvider(provider);
        //TODO
        builder;
}
