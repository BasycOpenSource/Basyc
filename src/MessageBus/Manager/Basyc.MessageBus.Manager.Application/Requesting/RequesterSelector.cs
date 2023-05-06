using Basyc.MessageBus.Manager.Application.Building;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class RequesterSelector : IRequesterSelector
{
    private readonly Dictionary<MessageInfo, string> infoToRequesterNameMap;
    private readonly Dictionary<string, IRequestHandler> requesterToChoose;

    public RequesterSelector(IEnumerable<IRequestHandler> requesters, IOptions<RequesterSelectorOptions> options)
    {
        //requesterToChoose = requesters.ToDictionary(x => x.UniqueName, x => x);
        requesterToChoose = new Dictionary<string, IRequestHandler>();
        foreach (var requester in requesters)
        {
            if (requesterToChoose.TryGetValue(requester.UniqueName, out var foundRequester))
            {
                if (foundRequester != requester)
                    throw new InvalidOperationException($"2 requesters with same unique name ({requester.UniqueName}) found");
            }
            else
            {
                requesterToChoose.Add(requester.UniqueName, requester);
            }
        }

        infoToRequesterNameMap = options.Value.ResolveRequesterMap();
    }

    public IRequestHandler PickRequester(MessageInfo requestInfo)
    {
        string requesterName = infoToRequesterNameMap[requestInfo];
        return requesterToChoose[requesterName];
    }

    public void AssignRequesterForMessage(MessageInfo requestInfo, string requesterUniqueName) => infoToRequesterNameMap.Add(requestInfo, requesterUniqueName);
}
