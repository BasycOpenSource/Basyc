using Basyc.MicroService.Abstraction.Initialization;
//using Dapr.Actors.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Basyc.MicroService.Asp.Dapr;

public class DaprMicroserviceProvider : IMicroserviceProvider
{
    public DaprMicroserviceProvider(IWebHostBuilder webBuilder)
    {
    }

    public void RegisterActor<TActor>() => RegisterActor(typeof(TActor));

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
