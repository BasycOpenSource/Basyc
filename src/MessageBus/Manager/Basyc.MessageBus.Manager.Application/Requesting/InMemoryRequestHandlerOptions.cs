using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryRequestHandlerOptions
{
	private readonly Dictionary<MessageInfo, Action<MessageRequest>> handlerMap = new();

	public void AddDelegateHandler(MessageInfo requestInfo, Action<MessageRequest> handler)
	{
		handlerMap.Add(requestInfo, handler);
	}

	public Dictionary<MessageInfo, Action<MessageRequest>> ResolveHandlers()
	{
		return handlerMap;
	}
}
