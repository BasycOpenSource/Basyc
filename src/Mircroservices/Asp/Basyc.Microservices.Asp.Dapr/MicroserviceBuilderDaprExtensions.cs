using Basyc.MicroService.Abstraction.Initialization;
//using Dapr.Actors.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MicroService.Asp.Dapr
{
    public static class MicroserviceBuilderDaprExtensions
    {
        public static MicroserviceBuilder<TParentBuilder> AddDaprProvider<TParentBuilder>(this MicroserviceBuilder<TParentBuilder> builder)
        {
            //var provider = new DaprMicroserviceProvider(builder.webBuilder);
            //builder.AddProvider(provider);
            //TODO
            return builder;
        }
    }
}