using Basyc.ReactiveUi;
using Basyc.ReactiveUi.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
public class NavigationService : BasycReactiveViewModelBase
{
    [Reactive] public RenderFragment? CurrentPage { get; private set; } = null;
    [Reactive] public object? CurrentQueryParams { get; private set; } = null;

    public void GoTo<TView, TViewModel, TQueryParams>(TQueryParams queryParams)
        where TView : BasycReactiveBlazorComponentBase<TViewModel, TQueryParams>
        where TViewModel : BasycReactiveViewModelBase
    {
        void currentPageRenderFragment(RenderTreeBuilder builder)
        {
            builder.OpenComponent<TView>(0);
            builder.AddAttribute(1, nameof(BasycReactiveBlazorComponentBase<TViewModel, TQueryParams>.QueryParams), queryParams);
            builder.CloseComponent();
        }
        CurrentQueryParams = queryParams;
        CurrentPage = currentPageRenderFragment;
    }

    public void GoTo<TView, TViewModel>()
    where TView : BasycReactiveBlazorComponentBase<TViewModel, object>
    where TViewModel : BasycReactiveViewModelBase
    {
        static void currentPageRenderFragment(RenderTreeBuilder builder)
        {
            builder.OpenComponent<TView>(0);
            builder.AddAttribute(1, nameof(BasycReactiveBlazorComponentBase<TViewModel, object>.QueryParams), new object());
            builder.CloseComponent();
        }
        CurrentQueryParams = new object();
        CurrentPage = currentPageRenderFragment;
    }

    public void Clear()
    {
        CurrentQueryParams = null;
        CurrentPage = null;
    }
}
