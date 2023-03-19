using Basyc.MessageBus.Manager.Application;
using Basyc.ReactiveUi;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message.Sidebar;
public class SidebarHistoryItemViewModel : BasycReactiveViewModelBase
{
	[Reactive] public MessageRequest? MessageRequest { get; set; }
	[Reactive] public RequestResultState State { get; init; }
	[Reactive] public TimeSpan Duration { get; init; }
	public SidebarHistoryItemViewModel()
	{
		State = this.ReactiveProperty(
			x => x.State,
			x => x.MessageRequest!.State);

		Duration = this.ReactiveProperty(
			x => x.Duration,
			x => x.MessageRequest!.Duration);
	}
}
