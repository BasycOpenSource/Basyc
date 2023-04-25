using ReactiveUI;

namespace Basyc.ReactiveUi;
public abstract class BasycReactiveViewModelBase : ReactiveObject, IBasycReactiveViewModel
{
    public List<IDisposable> Disposables { get; } = new();

    public virtual void Dispose() => Disposables.ForEach(x => x.Dispose());

    public void RaisePropertyChanged(string propertyName) => IReactiveObjectExtensions.RaisePropertyChanged(this, propertyName);
}
