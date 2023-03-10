using Basyc.MessageBus.Manager.Application.Initialization;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryRequestHandlerOptions
{
	private readonly Dictionary<RequestInfo, Action<RequestContext>> handlerMap = new();

	public void AddDelegateHandler(RequestInfo requestInfo, Action<RequestContext> handler)
	{
		handlerMap.Add(requestInfo, handler);
	}

	public Dictionary<RequestInfo, Action<RequestContext>> ResolveHandlers()
	{
		return handlerMap;
	}
}
