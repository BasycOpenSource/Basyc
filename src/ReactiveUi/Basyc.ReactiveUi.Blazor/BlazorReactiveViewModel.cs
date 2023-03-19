using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Blazor;
using System.ComponentModel;

namespace Basyc.ReactiveUi.Blazor;
public class BasycReactiveBlazorComponentBase<TViewModel> : ReactiveComponentBase<TViewModel>
	where TViewModel : class, INotifyPropertyChanged
{
	[Inject] private IServiceProvider? services { get; set; }
	public new TViewModel ViewModel => base.ViewModel!;

	public BasycReactiveBlazorComponentBase()
	{
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.ViewModel = services.Value().GetRequiredService<TViewModel>();
	}
}
