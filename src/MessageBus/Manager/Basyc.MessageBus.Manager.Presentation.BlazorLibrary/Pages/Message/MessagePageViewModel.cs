using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
using Basyc.ReactiveUi;
using Microsoft.AspNetCore.Components;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message;
public class MessagePageViewModel : BasycReactiveViewModelBase
{
    public NavigationService NavigationService { get; init; } = new NavigationService();

    [Reactive] public RenderFragment? PageToDisplay { get; init; }
    public MessagePageViewModel()
    {
        PageToDisplay = this.ReactiveProperty(x => x.PageToDisplay,
            x => x.NavigationService.CurrentPage);
    }
}
