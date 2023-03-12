using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public interface IRequestManager
{
	MessageRequest StartRequest(RequestInput request);
	//Dictionary<MessageInfo, List<RequestContext>> Requests { get; }
	ReadOnlyCollection<MessageContext> Requests { get; }
}
