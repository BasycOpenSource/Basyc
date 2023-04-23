using System.Threading;

namespace Basyc.MessageBus.Client;

public class InMemorySharedRequestIdCounter : ISharedRequestIdCounter
{
    private int counter;
    public int GetLastId() => counter;

    public int GetNextId() => Interlocked.Increment(ref counter);
}
