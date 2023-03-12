using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public class DomainItemViewModel
{
	public DomainItemViewModel(DomainInfo requestDomainInfo, IEnumerable<SidebarMessageItemViewModel> requestViewModels)
	{
		RequestDomainInfo = requestDomainInfo;
		RequestItemViewModels = new List<SidebarMessageItemViewModel>(requestViewModels);
	}

	public DomainInfo RequestDomainInfo { get; }
	public IReadOnlyCollection<SidebarMessageItemViewModel> RequestItemViewModels { get; }
}
