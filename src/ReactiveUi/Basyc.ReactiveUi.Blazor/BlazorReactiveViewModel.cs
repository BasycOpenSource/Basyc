using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Blazor;
using System.ComponentModel;

namespace Basyc.ReactiveUi.Blazor;
public abstract class BasycReactiveBlazorComponentBase<TViewModel, TQueryParams> : ReactiveComponentBase<TViewModel>
    where TViewModel : class, INotifyPropertyChanged
{
    [Inject] private IServiceProvider? services { get; set; }
    public new TViewModel ViewModel => base.ViewModel!;

    [Parameter]
    public TQueryParams QueryParams { get; set; } = default!;

    public BasycReactiveBlazorComponentBase()
    {
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        base.ViewModel = services.Value().GetRequiredService<TViewModel>();
        OnVisit(QueryParams);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        //base.ViewModel = services.Value().GetRequiredService<TViewModel>();
        OnVisit(QueryParams);
    }

    public abstract void OnVisit(TQueryParams queryParams);
}

public abstract class BasycReactiveBlazorComponentBase<TViewModel> : BasycReactiveBlazorComponentBase<TViewModel, object>
    where TViewModel : class, INotifyPropertyChanged
{
    public override void OnVisit(object queryParams)
    {

    }
}
