using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
using Basyc.ReactiveUi;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.SideBar;
public class AppSideBarViewModel : BasycReactiveViewModelBase
{
	private readonly NavigationService navigationService;

	public AppSideBarViewModel(NavigationService navigationService)
	{
		this.navigationService = navigationService;
	}

	public void SelectMessage(MessageInfo messageInfo)
	{
		navigationService.GoTo<MessagePageView, MessagePageViewModel, MessageInfo>(messageInfo);
	}
}

