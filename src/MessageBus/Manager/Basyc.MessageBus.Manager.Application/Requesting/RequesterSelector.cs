using Basyc.MessageBus.Manager.Application.Initialization;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class RequesterSelector : IRequesterSelector
{
	private readonly Dictionary<RequestInfo, string> infoToRequesterNameMap;
	private readonly IOptions<RequesterSelectorOptions> options;
	private readonly Dictionary<string, IRequestHandler> requesterToChoose;

	public RequesterSelector(IEnumerable<IRequestHandler> requesters, IOptions<RequesterSelectorOptions> options)
	{
		//requesterToChoose = requesters.ToDictionary(x => x.UniqueName, x => x);
		requesterToChoose = new Dictionary<string, IRequestHandler>();
		foreach (var requester in requesters)
			if (requesterToChoose.TryGetValue(requester.UniqueName, out var foundRequester))
			{
				if (foundRequester != requester)
					throw new Exception($"2 requesters with same unique name ({requester.UniqueName}) found");
			}
			else
				requesterToChoose.Add(requester.UniqueName, requester);

		this.options = options;
		infoToRequesterNameMap = options.Value.ResolveRequesterMap();
	}

	public IRequestHandler PickRequester(RequestInfo requestInfo)
	{
		var requesterName = infoToRequesterNameMap[requestInfo];
		return requesterToChoose[requesterName];
	}

	public void AssignRequester(RequestInfo requestInfo, string requesterUniqueName)
	{
		infoToRequesterNameMap.Add(requestInfo, requesterUniqueName);
	}
}
