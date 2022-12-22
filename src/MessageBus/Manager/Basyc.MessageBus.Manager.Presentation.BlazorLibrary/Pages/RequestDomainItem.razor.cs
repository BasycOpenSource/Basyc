using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public partial class RequestDomainItem
{
    [Parameter]
    public DomainItemViewModel DomainItemViewModel { get; set; }

    [Parameter]
    public EventCallback<RequestItem> OnMessageSending { get; set; }
}
