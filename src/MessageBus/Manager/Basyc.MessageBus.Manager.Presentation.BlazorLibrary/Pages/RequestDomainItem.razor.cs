using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public partial class RequestDomainItem
{
	[Parameter][EditorRequired] public DomainItemViewModel DomainItemViewModel { get; set; } = null!;

	[Parameter] public EventCallback<RequestItem> OnMessageSending { get; set; }
}

