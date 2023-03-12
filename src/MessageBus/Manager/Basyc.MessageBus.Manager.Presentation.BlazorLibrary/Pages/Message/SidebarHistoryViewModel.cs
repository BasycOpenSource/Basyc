using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Requesting;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message;
public class SidebarHistoryViewModel : ReactiveObject
{
	private readonly IRequestManager requestManager;

	[Reactive]
	public ReadOnlyCollection<MessageContext> History { get; init; } = null!;
	public SidebarHistoryViewModel(IRequestManager requestManager)
	{
		this.requestManager = requestManager;
		requestManager.Requests.AsObservableChangeSet()
			.Bind(out var history);
		History = history;
	}
}
