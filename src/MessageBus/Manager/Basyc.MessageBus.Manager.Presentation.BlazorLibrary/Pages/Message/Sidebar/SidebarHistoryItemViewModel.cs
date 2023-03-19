using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
using Basyc.ReactiveUi;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message.Sidebar;
public class SidebarHistoryItemViewModel : BasycReactiveViewModelBase
{
	[Reactive] public MessageRequest? MessageRequest { get; set; }
	[Reactive] public RequestResultState State { get; init; }
	[Reactive] public TimeSpan Duration { get; init; }
	[Reactive] public int LogsCount { get; init; }
	[Reactive] public NavigationService? NavigationService { get; set; }
	[Reactive] public bool IsSelected { get; init; }

	public SidebarHistoryItemViewModel()
	{
		State = this.ReactiveProperty(
			x => x.State,
			x => x.MessageRequest!.State);

		Duration = this.ReactiveProperty(
			x => x.Duration,
			x => x.MessageRequest!.Duration);

		LogsCount = this.ReactiveProperty(
			x => x.LogsCount,
			x => x.MessageRequest!.Diagnostics.LogEntries.Count);

		IsSelected = this.ReactiveProperty(
			x => x.IsSelected,
			x => x.NavigationService!.CurrentQueryParams,
			x => x.MessageRequest,
			x => x.Item1 is MessageRequest messageRequest && messageRequest == x.Item2);
	}
}
