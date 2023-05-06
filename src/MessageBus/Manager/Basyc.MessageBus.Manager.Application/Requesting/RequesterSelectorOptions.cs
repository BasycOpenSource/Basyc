using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class RequesterSelectorOptions
{
    private readonly Dictionary<MessageInfo, string> requesterNameToRequestInfoMap = new();

    public void AssignRequester(MessageInfo requestInfo, string requesterUniqueName) => requesterNameToRequestInfoMap.Add(requestInfo, requesterUniqueName);

    public Dictionary<MessageInfo, string> ResolveRequesterMap() => requesterNameToRequestInfoMap;
}
