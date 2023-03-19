namespace Basyc.ReactiveUi;
public class ReactiveSubscription : IDisposable
{
	private readonly IDisposable subscription;

	public ReactiveSubscription(IDisposable subscription)
	{
		this.subscription = subscription;
	}

	public void Unsubscribe()
	{
		Dispose();
	}

	public void Dispose()
	{
		subscription.Dispose();
	}
}

