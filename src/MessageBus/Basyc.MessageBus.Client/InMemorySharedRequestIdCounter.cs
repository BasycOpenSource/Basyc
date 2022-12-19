using System.Threading;

namespace Basyc.MessageBus.Client
{
    public class InMemorySharedRequestIdCounter : ISharedRequestIdCounter
    {
        private int counter;
        public int GetLastId()
        {
            return counter;
        }

        public int GetNextId()
        {
            return Interlocked.Increment(ref counter);
        }
    }
}