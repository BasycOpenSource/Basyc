using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Blazor;
using System.ComponentModel;

namespace Basyc.ReactiveUi.Blazor;
public abstract class BasycReactiveBlazorComponentBase<TViewModel, TQueryParams>
	: ReactiveComponentBase<TViewModel>, IBasycReactiveViewModel, IBasycReactiveBlazorComponentBase, IReactiveObject
	where TViewModel : class, INotifyPropertyChanged
{
	[Inject] private IServiceProvider? services { get; set; }
	[Parameter] public TQueryParams QueryParams { get; set; } = default!;

	public new TViewModel ViewModel => base.ViewModel!;
	public List<IDisposable> Disposables { get; } = new();
	public event PropertyChangingEventHandler? PropertyChanging;

	protected BasycReactiveBlazorComponentBase()
	{
		this.SubscribePropertyChangedEvents();
		PropertyChanged += PropertyChangedHandler;

	}

	private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
	{
		InvokeAsync(StateHasChanged);
	}

	protected override void OnInitialized()
	{
		if (typeof(TViewModel).IsAssignableTo(typeof(INullViewModel)) is false)
			base.ViewModel = services.Value().GetRequiredService<TViewModel>();
		OnVisit(QueryParams);
		base.OnInitialized();
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		//base.ViewModel = services.Value().GetRequiredService<TViewModel>();
		OnVisit(QueryParams);
	}

	//TODO: maybe dont call in OnInitialized/OnParametersSet but only in navigation service?
	public abstract void OnVisit(TQueryParams queryParams);

	public void RaisePropertyChanging(PropertyChangingEventArgs args)
	{
		PropertyChanging?.Invoke(this, args);
	}

	public void RaisePropertyChanged(PropertyChangedEventArgs args)
	{
		base.OnPropertyChanged(args.PropertyName);
	}
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
	protected override void OnInitialized()
	{
		base.OnInitialized();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
	}
}

