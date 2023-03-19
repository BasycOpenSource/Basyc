using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.SideBar;

public partial class AppSideBarView
{
	[Inject] private IMessagesProvider DomainInfoProviderManager { get; set; } = null!;
	private ReadOnlyObservableCollection<MessageGroup> messageGroups = null!;

	protected override void OnInitialized()
	{
		messageGroups = new ReadOnlyObservableCollection<MessageGroup>(new ObservableCollection<MessageGroup>(DomainInfoProviderManager.GetMessageGroups()));
		base.OnInitialized();
	}
}
