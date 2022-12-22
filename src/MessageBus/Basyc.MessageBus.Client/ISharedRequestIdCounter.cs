namespace Basyc.MessageBus.Client;

public interface ISharedRequestIdCounter
{
    int GetLastId();
    int GetNextId();
}
