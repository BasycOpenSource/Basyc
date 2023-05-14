using Basyc.ReactiveUi;
using Basyc.ReactiveUi.Blazor;
using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
public interface INavigationService
{
    RenderFragment? CurrentPage { get; }

    object? CurrentQueryParams { get; }

    void Clear();

    void GoTo<TView, TViewModel, TQueryParams>(TQueryParams queryParams)
        where TView : BasycReactiveBlazorComponentBase<TViewModel, TQueryParams>
        where TViewModel : BasycReactiveViewModelBase;

    void GoTo<TView, TViewModel>()
        where TView : BasycReactiveBlazorComponentBase<TViewModel, object>
        where TViewModel : BasycReactiveViewModelBase;
}
