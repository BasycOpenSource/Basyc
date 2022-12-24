using Basyc.MessageBus.Manager.Application.Initialization;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public interface IRequesterSelector
{
	void AssignRequester(RequestInfo requestInfo, string requesterUniqueName);
	IRequester PickRequester(RequestInfo requestInfo);
}
