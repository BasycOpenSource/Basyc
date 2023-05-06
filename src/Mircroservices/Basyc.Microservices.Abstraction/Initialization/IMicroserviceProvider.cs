namespace Basyc.MicroService.Abstraction.Initialization;

public interface IMicroserviceProvider
{
    void RegisterActor<TActor>();

    void RegisterActor(Type actorType);
}
