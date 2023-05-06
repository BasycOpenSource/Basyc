using ReactiveUI;

namespace Basyc.ReactiveUi;
#pragma warning disable CA1002 // Do not expose generic lists

public interface IBasycReactiveViewModel : IReactiveObject, IDisposable
{
    /// <summary>
    /// Object and subscriptions that need to be disposed together with <see cref="IBasycReactiveViewModel"/>.
    /// </summary>
    List<IDisposable> Disposables { get; }
}
