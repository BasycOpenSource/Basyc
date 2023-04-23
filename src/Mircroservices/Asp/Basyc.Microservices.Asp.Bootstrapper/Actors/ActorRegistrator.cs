using Basyc.MicroService.Abstraction.Initialization;
using Dapr.Actors;
using System.Reflection;

namespace Basyc.MicroService.Asp.Bootstrapper.Actors;

public class ActorRegistrator
{
    private readonly IMicroserviceProvider microserviceProvider;

    public ActorRegistrator(IMicroserviceProvider microserviceProvider)
    {
        this.microserviceProvider = microserviceProvider;
    }

    /// <summary>
    ///     <typeparamref name="TStartup" /> must be in default namespace besides Actors folder
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public void RegisterActors<TStartup>() => RegisterActors(typeof(TStartup).Namespace + "/Actors");

    public void RegisterActors(string actorsNamespace)
    {
        var actorTypes = Assembly.GetEntryAssembly()!.DefinedTypes.Where(x =>
            x.IsClass && x.ImplementedInterfaces.Contains(typeof(IActor)) && x.Namespace!.StartsWith(actorsNamespace));
        foreach (var actor in actorTypes)
        {
            microserviceProvider.RegisterActor(actor);
        }
    }
}
