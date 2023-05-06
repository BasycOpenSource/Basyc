using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryRequestHandlerOptions
{
    private readonly Dictionary<MessageInfo, RequestHandler> handlerMap = new();

    public void AddDelegateHandler(MessageInfo requestInfo, RequestHandler handler) => handlerMap.Add(requestInfo, handler);

    public Dictionary<MessageInfo, RequestHandler> ResolveHandlers() => handlerMap;
}
