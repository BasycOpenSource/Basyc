using Basyc.MessageBus.Manager.Application;
using Basyc.ReactiveUi;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message;
public class MessagePageRequestPageViewModel : BasycReactiveViewModelBase
{
    public MessagePageRequestPageViewModel()
    {
    }

    [Reactive] public MessageRequest? MessageRequest { get; set; }
}
