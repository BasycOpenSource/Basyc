using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Blazor;
using System.ComponentModel;

namespace Basyc.ReactiveUi.Blazor;
#pragma warning disable SA1402

public abstract class BasycReactiveBlazorComponentBase<TViewModel, TQueryParams>
    : ReactiveComponentBase<TViewModel>, IBasycReactiveViewModel, IBasycReactiveBlazorComponentBase, IReactiveObject
    where TViewModel : class, INotifyPropertyChanged
{
    protected BasycReactiveBlazorComponentBase()
    {
        this.SubscribePropertyChangedEvents();
        PropertyChanged += PropertyChangedHandler;
    }

    public event PropertyChangingEventHandler? PropertyChanging;

    [Parameter] public TQueryParams QueryParams { get; set; } = default!;

    public new TViewModel ViewModel => base.ViewModel!;

    public List<IDisposable> Disposables { get; } = new();

    [Inject] private IServiceProvider? Services { get; set; }

    //TODO: maybe dont call in OnInitialized/OnParametersSet but only in navigation service?
    public abstract void OnVisit(TQueryParams queryParams);

    public void RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    public void RaisePropertyChanged(PropertyChangedEventArgs args) => OnPropertyChanged(args.PropertyName);

    protected override void OnInitialized()
    {
        if (typeof(TViewModel).IsAssignableTo(typeof(INullViewModel)) is false)
            base.ViewModel = Services.Value().GetRequiredService<TViewModel>();
        OnVisit(QueryParams);
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        //base.ViewModel = services.Value().GetRequiredService<TViewModel>();
        OnVisit(QueryParams);
    }

    private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e) => InvokeAsync(StateHasChanged);
}

public abstract class BasycReactiveBlazorComponentBase<TViewModel> : BasycReactiveBlazorComponentBase<TViewModel, INullQueryParameters>
    where TViewModel : class, INotifyPropertyChanged
{
    public override void OnVisit(INullQueryParameters queryParams)
    {
    }
}

public abstract class BasycReactiveBlazorComponentBase : BasycReactiveBlazorComponentBase<INullViewModel>
{
    protected override void OnInitialized() => base.OnInitialized();

    protected override void OnAfterRender(bool firstRender) => base.OnAfterRender(firstRender);

    protected override void OnParametersSet() => base.OnParametersSet();
}
