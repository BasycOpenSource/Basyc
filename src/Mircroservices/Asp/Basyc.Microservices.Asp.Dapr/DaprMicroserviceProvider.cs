using Basyc.MicroService.Abstraction.Initialization;
//using Dapr.Actors.AspNetCore;
using Dapr.Actors.Runtime;
using Dapr.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basyc.MicroService.Asp.Dapr;

public class DaprMicroserviceProvider : IMicroserviceProvider
{
    private readonly IWebHostBuilder webBuilder;

    public DaprMicroserviceProvider(IWebHostBuilder webBuilder)
    {
        this.webBuilder = webBuilder;
    }

    public void RegisterActor<TActor>()
    {
        RegisterActor(typeof(TActor));
    }

    public void RegisterActor(Type actorType)
    {
        //webBuilder.UseActors(x =>
        //{
        //    ActorTypeInformation actorTypeInfo = ActorTypeInformation.Get(actorType);
        //    var registration = new ActorRegistration(actorTypeInfo);
        //    x.Actors.Add(registration);
        //});
    }
}
