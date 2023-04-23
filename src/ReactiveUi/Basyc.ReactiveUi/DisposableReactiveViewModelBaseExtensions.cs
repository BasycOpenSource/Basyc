namespace Basyc.ReactiveUi;
public static class DisposableReactiveViewModelBaseExtensions
{
    public static TDisposable DisposeWithViewModel<TDisposable>(this TDisposable disposable, BasycReactiveViewModelBase viewModel)
        where TDisposable : IDisposable
    {
        viewModel.Disposables.Add(disposable);
        return disposable;
    }

    public static TDisposable DisposeWithViewModel<TDisposable>(this TDisposable disposable, BasycReactiveViewModelBase viewModel, IDisposable? oldDisposable)
        where TDisposable : IDisposable
    {
        if (oldDisposable is not null)
        {
            oldDisposable.Dispose();
            viewModel.Disposables.Remove(oldDisposable);
        }

        return disposable.DisposeWithViewModel(viewModel);
    }

    public static ReactiveSubscription ToReactiveSubscription(this IDisposable subscription) => new ReactiveSubscription(subscription);
}
