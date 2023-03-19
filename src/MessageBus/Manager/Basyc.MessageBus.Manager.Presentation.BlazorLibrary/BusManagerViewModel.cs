using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
using Basyc.ReactiveUi;
using Microsoft.AspNetCore.Components;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary;
public class BusManagerViewModel : BasycReactiveViewModelBase
{
	private readonly NavigationService navigationService;
	[Reactive] public RenderFragment? CurrentPage { get; private set; }

	public BusManagerViewModel(NavigationService navigationService)
	{
		this.navigationService = navigationService;
		CurrentPage = this.ReactiveProperty(x => x.CurrentPage, x => x.navigationService.CurrentPage);
	}
}
