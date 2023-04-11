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

	public abstract void OnVisit(TQueryParams queryParams);

	public void RaisePropertyChanging(PropertyChangingEventArgs args)
	{
		PropertyChanging?.Invoke(this, args);
	}

	public void RaisePropertyChanged(string propertyName)
	{
		base.OnPropertyChanged(propertyName);
	}

	public void RaisePropertyChanged(PropertyChangedEventArgs args)
	{
		base.OnPropertyChanged(args.PropertyName);
	}
}

public abstract class BasycReactiveBlazorComponentBase<TViewModel> : BasycReactiveBlazorComponentBase<TViewModel, object>
	where TViewModel : class, INotifyPropertyChanged
{
	public override void OnVisit(object queryParams)
	{

	}
}

public interface INullViewModel : INotifyPropertyChanged
{
}

public interface IBasycReactiveBlazorComponentBase : INullViewModel
{
}

public abstract class BasycReactiveBlazorComponentBase : BasycReactiveBlazorComponentBase<IBasycReactiveBlazorComponentBase, object>
{
	public BasycReactiveBlazorComponentBase()
	{
		var asReactiveComponentBase = (ReactiveComponentBase<IBasycReactiveBlazorComponentBase>)this;
		PropertyChanged += PropertyChangedHandler;
		asReactiveComponentBase.ViewModel = this;
		//this.WhenAnyValue(x => x.ViewModel)
		//	.Subscribe(x =>
		//	{
		//		x.PropertyChanged += PropertyChangedHandler;
		//	});
	}

	private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
	{
		//StateHasChanged();
	}

	public override void OnVisit(object queryParams)
	{

	}

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
