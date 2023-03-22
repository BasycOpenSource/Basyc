using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryRequestHandlerOptions
{
	private readonly Dictionary<MessageInfo, RequestHandlerDelegate> handlerMap = new();

	public void AddDelegateHandler(MessageInfo requestInfo, RequestHandlerDelegate handler)
	{
		handlerMap.Add(requestInfo, handler);
	}

	public Dictionary<MessageInfo, RequestHandlerDelegate> ResolveHandlers()
	{
		return handlerMap;
	}
}
