using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public interface IRequesterSelector
{
    void AssignRequesterForMessage(MessageInfo requestInfo, string requesterUniqueName);

    IRequestHandler PickRequester(MessageInfo requestInfo);
}
