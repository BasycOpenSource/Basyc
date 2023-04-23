using ReactiveUI;

namespace Basyc.ReactiveUi;
public abstract class BasycReactiveViewModelBase : ReactiveObject, IReactiveViewModel
{
    public virtual void Dispose() => Disposables.ForEach(x => x.Dispose());

    public List<IDisposable> Disposables { get; } = new();
}
