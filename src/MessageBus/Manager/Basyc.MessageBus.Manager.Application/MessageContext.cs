using Basyc.MessageBus.Manager.Application.Building;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Application;
public class MessageContext : ReactiveObject
{
    public MessageContext(MessageInfo messageInfo)
    {
        MessageInfo = messageInfo;
    }

    [Reactive] public ObservableCollection<MessageRequest> MessageRequests { get; init; } = new();

    [Reactive] public MessageInfo MessageInfo { get; init; }
}
