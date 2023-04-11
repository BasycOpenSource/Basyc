using ReactiveUI;

namespace Basyc.ReactiveUi;
public interface IBasycReactiveViewModel : IReactiveObject, IDisposable
{
	/// <summary>
	/// Object and subscriptions that need to be disposed together with <see cref="IBasycReactiveViewModel"/>
	/// </summary>
	List<IDisposable> Disposables { get; }
	void RaisePropertyChanged(string propertyName);
}

