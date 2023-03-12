using Basyc.MessageBus.Manager.Application.Building;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Application;
public class MessageContext : ReactiveObject
{
	public MessageContext(MessageInfo messageInfo)
	{
		MessageInfo = messageInfo;
	}

	public ObservableCollection<MessageRequest> MessageRequests { get; } = new();
	public MessageInfo MessageInfo { get; }
}
