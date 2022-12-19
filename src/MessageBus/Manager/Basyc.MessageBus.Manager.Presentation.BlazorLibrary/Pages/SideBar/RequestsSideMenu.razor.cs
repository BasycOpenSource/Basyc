using Basyc.MessageBus.Manager.Application;
using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.SideBar;

public partial class RequestsSideMenu
{
	[Parameter] public List<DomainItemViewModel> DomainInfoViewModel { get; set; } = new List<DomainItemViewModel>();
	[Parameter] public EventCallback<RequestItemViewModel> RequestSelected { get; set; }


}
