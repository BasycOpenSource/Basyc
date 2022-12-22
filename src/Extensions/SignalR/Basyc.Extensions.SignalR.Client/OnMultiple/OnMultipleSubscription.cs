namespace Basyc.Extensions.SignalR.Client.OnMultiple;

public class OnMultipleSubscription : IDisposable
{
    private readonly IDisposable[] innerSubscriptions;

    public OnMultipleSubscription(IDisposable[] innerSubscriptions)
    {
        this.innerSubscriptions = innerSubscriptions;
    }

    /// <summary>
    /// Unsubscribes all methods from connection
    /// </summary>
    public void UnsubscribeAll()
    {
        foreach (var innerSubscription in innerSubscriptions)
        {
            innerSubscription.Dispose();
        }
    }

    public void Dispose()
    {
        UnsubscribeAll();
    }
}
