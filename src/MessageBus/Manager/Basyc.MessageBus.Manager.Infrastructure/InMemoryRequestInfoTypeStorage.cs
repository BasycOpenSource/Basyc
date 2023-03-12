using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class InMemoryRequestInfoTypeStorage : IRequestInfoTypeStorage
{
	private readonly Dictionary<MessageInfo, Type> storage = new();
	public void AddRequest(MessageInfo requestInfo, Type requestType)
	{
		storage.Add(requestInfo, requestType);
	}

	public Type GetRequestType(MessageInfo requestInfo)
	{
		return storage[requestInfo];
	}
}
