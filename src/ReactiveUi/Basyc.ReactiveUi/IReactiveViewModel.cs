namespace Basyc.ReactiveUi;
public interface IReactiveViewModel : IDisposable
{
	/// <summary>
	/// Object and subscriptions that need to be disposed together with <see cref="IReactiveViewModel"/>
	/// </summary>
	List<IDisposable> Disposables { get; }
}

