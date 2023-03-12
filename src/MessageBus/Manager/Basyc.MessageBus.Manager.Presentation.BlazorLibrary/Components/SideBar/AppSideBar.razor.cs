using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.SideBar;

public partial class AppSideBar
{
	[Parameter] public EventCallback<MessageInfo> RequestSelected { get; set; }
	[Inject] private IDomainInfoProviderManager DomainInfoProviderManager { get; set; } = null!;
	private ReadOnlyCollection<DomainItemViewModel> domainInfoViewModel = new(Array.Empty<DomainItemViewModel>());

	protected override void OnInitialized()
	{
		domainInfoViewModel = DomainInfoProviderManager.GetDomainInfos()
			.Select(domainInfo => new DomainItemViewModel(domainInfo, domainInfo.Requests
				.Select(requestInfo => new SidebarMessageItemViewModel(requestInfo))
				.OrderBy(x => x.RequestInfo.RequestType)))
			.ToList().AsReadOnly();

		base.OnInitialized();
	}
}
